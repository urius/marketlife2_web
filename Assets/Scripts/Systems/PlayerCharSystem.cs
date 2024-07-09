using System;
using System.Linq;
using Commands;
using Data;
using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.BuildPoint;
using Model.ShopObjects;
using UnityEngine;

namespace Systems
{
    public class PlayerCharSystem : ISystem
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ICommandExecutor _commandExecutor = Instance.Get<ICommandExecutor>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        
        private ShopModel _shopModel;
        private PlayerModel _playerModel;
        private PlayerCharModel _playerCharModel;
        private bool _putProductsIsBlocked;

        public void Start()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            _shopModel = _playerModel.ShopModel;
            _playerCharModel = _playerModel.PlayerCharModel;
            
            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<RequestPlayerCellChangeEvent>(OnRequestPlayerCellChanged);
            _eventBus.Subscribe<SpendMoneyOnBuildPointAnimationHalfEvent>(OnSpendMoneyOnBuildPointHalfAnimation);
            _eventBus.Subscribe<SpendMoneyOnBuildPointAnimationFinishedEvent>(OnSpendMoneyOnBuildPointAnimationFinished);
            _eventBus.Subscribe<TruckArrivedEvent>(OnTruckArrivedEvent);
            _eventBus.Subscribe<PutProductOnShelfHalfAnimationEvent>(OnPutProductOnShelfHalfAnimationEvent);
            _updatesProvider.QuarterSecondPassed += OnQuarterSecondPassed;
            _playerCharModel.CellPositionChanged += OnCellPositionChanged;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<RequestPlayerCellChangeEvent>(OnRequestPlayerCellChanged);
            _eventBus.Unsubscribe<SpendMoneyOnBuildPointAnimationHalfEvent>(OnSpendMoneyOnBuildPointHalfAnimation);
            _eventBus.Unsubscribe<SpendMoneyOnBuildPointAnimationFinishedEvent>(OnSpendMoneyOnBuildPointAnimationFinished);
            _eventBus.Unsubscribe<TruckArrivedEvent>(OnTruckArrivedEvent);
            _eventBus.Unsubscribe<PutProductOnShelfHalfAnimationEvent>(OnPutProductOnShelfHalfAnimationEvent);
            _updatesProvider.QuarterSecondPassed -= OnQuarterSecondPassed;
            _playerCharModel.CellPositionChanged -= OnCellPositionChanged;
        }

        private void OnRequestPlayerCellChanged(RequestPlayerCellChangeEvent e)
        {
            _playerCharModel.SetCellPosition(e.CellCoords);
        }

        private void OnQuarterSecondPassed()
        {
            if (_playerCharModel.NearCashDesk is { MoneyAmount: > 0 } 
                && IsPlayerNearTheBottomPartOfCashDesk())
            {
                var moneyAmount = _playerCharModel.NearCashDesk.MoneyAmount;
                
                _playerCharModel.NearCashDesk.ResetMoney();
                _playerModel.ChangeMoney(moneyAmount);
                
                _eventBus.Dispatch(new AnimateTakeMoneyFromCashDeskEvent(_playerCharModel.NearCashDesk, moneyAmount));
            }
        }

        private bool IsPlayerNearTheBottomPartOfCashDesk()
        {
            return _playerCharModel.CellPosition.y >= _playerCharModel.NearCashDesk.CellCoords.y;
        }

        private void OnCellPositionChanged(Vector2Int cellPosition)
        {
            TriggerSpendOnBuildPointIterationAnimationIfNeeded(cellPosition);
            TakeTruckProductBoxIfNeeded(cellPosition);
            PutProductOnShelfIfNeeded(cellPosition);
            CheckNearCashDesk(cellPosition);
        }

        private void CheckNearCashDesk(Vector2Int cellPosition)
        {
            foreach (var cellOffset in Constants.NearCells8)
            {
                var nearCell = cellPosition + cellOffset;

                if (_ownedCellsDataHolder.TryGetCashDesk(nearCell, out var cashDeskModel))
                {
                    _playerCharModel.SetNearCashDesk(cashDeskModel);
                    return;
                }
            }

            _playerCharModel.ResetNearCashDesk();
        }

        private void PutProductOnShelfIfNeeded(Vector2Int cellPosition)
        {
            if (!_playerCharModel.HasProducts || _putProductsIsBlocked) return;

            var nearEmptyShelf = GetNearEmptyShelf(cellPosition);

            if (nearEmptyShelf != null)
            {
                _putProductsIsBlocked = true;
                
                var shelfSlotIndex = nearEmptyShelf.GetEmptySlotIndex();

                var boxSlotIndex = _playerCharModel.GetNextNotEmptySlotIndex();
                var productToMove = _playerCharModel.ProductsInBox[boxSlotIndex];
                
                _playerCharModel.RemoveProductFromSlot(boxSlotIndex);

                nearEmptyShelf.AddProductToSlot(shelfSlotIndex, productToMove);
                
                _eventBus.Dispatch(
                    new AnimatePutProductOnShelfEvent(nearEmptyShelf, productToMove, boxSlotIndex, shelfSlotIndex));
            }
        }

        private void OnPutProductOnShelfHalfAnimationEvent(PutProductOnShelfHalfAnimationEvent e)
        {
            _putProductsIsBlocked = false;
            
            PutProductOnShelfIfNeeded(_playerCharModel.CellPosition);
        }

        private ShelfModel GetNearEmptyShelf(Vector2Int cellPosition)
        {
            foreach (var cellOffset in Constants.NearCells8)
            {
                var cellToCheck = cellPosition + cellOffset;
                if (_shopModel.TryGetShelfModel(cellToCheck, out var shelfModel)
                    && shelfModel.HasEmptySlots())
                {
                    return shelfModel;
                }
            }

            return null;
        }

        private void OnTruckArrivedEvent(TruckArrivedEvent e)
        {
            TakeTruckProductBoxIfNeeded(_playerCharModel.CellPosition);
        }

        private void TakeTruckProductBoxIfNeeded(Vector2Int cellPos)
        {
            if (cellPos.x == 0
                && (_shopModel.TryGetTruckPoint(new Vector2Int(-1, cellPos.y), out var truckPointModel)
                    || _shopModel.TryGetTruckPoint(new Vector2Int(-1, cellPos.y + 1), out truckPointModel)))
            {
                if (truckPointModel.DeliverTimeSecondsRest <= 0
                    && truckPointModel.HasProducts
                    && _playerCharModel.HasProducts == false)
                {
                    var productBoxIndexToTake = truckPointModel.GetFirstNotEmptyProductBoxIndex();
                    var productTypeToTake = truckPointModel.GetProductTypeAtBoxIndex(productBoxIndexToTake);
                    var productsToAdd = Enumerable.Repeat(productTypeToTake, Constants.ProductsAmountInBox).ToArray();
                    
                    truckPointModel.RemoveBox(productBoxIndexToTake);
                    _playerCharModel.SetProductsInBox(productsToAdd);
                    
                    _eventBus.Dispatch(new AnimateTakeBoxFromTruckEvent(truckPointModel, productBoxIndexToTake));
                }
            }
        }

        private void OnSpendMoneyOnBuildPointHalfAnimation(SpendMoneyOnBuildPointAnimationHalfEvent e)
        {
            if (_shopModel.BuildPoints.TryGetValue(e.TargetBuildPointCellCoords, out var buildPoint))
            {
                if (buildPoint.MoneyToBuildLeft > e.ActiveAnimationsAmount)
                {
                    TriggerSpendOnBuildPointIterationAnimationIfNeeded(_playerCharModel.CellPosition);
                }
            }
        }

        private void OnSpendMoneyOnBuildPointAnimationFinished(SpendMoneyOnBuildPointAnimationFinishedEvent e)
        {
            if (_shopModel.BuildPoints.TryGetValue(e.TargetBuildPointCellCoords, out var buildPoint))
            {
                buildPoint.ChangeMoneyToBuildLeft(-e.MoneyAmount);
            
                if (buildPoint.MoneyToBuildLeft <= 0)
                {
                    _commandExecutor.Execute<BuildShopObjectCommand, BuildPointModel>(buildPoint);
                }
            }
        }

        private void TriggerSpendOnBuildPointIterationAnimationIfNeeded(Vector2Int cellPosition)
        {
            if (_shopModel.BuildPoints.TryGetValue(cellPosition, out var buildPoint)
                && buildPoint.MoneyToBuildLeft > 0
                && _playerModel.Money > 0)
            {
                var moneyAmount = GetMoneyPerSingleAnimation(buildPoint.MoneyToBuildLeft);
                
                _playerModel.ChangeMoney(-moneyAmount);

                _eventBus.Dispatch(new TriggerSpendMoneyOnBuildPointAnimationEvent(buildPoint, moneyAmount));
            }
        }

        private int GetMoneyPerSingleAnimation(int moneyToBuildLeft)
        {
            var result = moneyToBuildLeft switch
            {
                > 500 => 300,
                > 100 => 50,
                > 50 => 25,
                > 20 => 10,
                > 10 => 5,
                > 5 => 3,
                _ => 1
            };

            return Math.Min(result, _playerModel.Money);
        }
    }
}