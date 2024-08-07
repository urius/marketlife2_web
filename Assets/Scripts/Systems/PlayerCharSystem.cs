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
using View.Helpers;

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
            _eventBus.Subscribe<SpendMoneyOnBuildPointLastAnimationFinishedEvent>(OnSpendMoneyOnBuildPointLastAnimationFinished);
            _eventBus.Subscribe<TruckArrivedEvent>(OnTruckArrivedEvent);
            _eventBus.Subscribe<PutProductOnShelfHalfAnimationEvent>(OnPutProductOnShelfHalfAnimationEvent);
            _updatesProvider.SecondPassed += OnSecondPassed;
            _updatesProvider.QuarterSecondPassed += OnQuarterSecondPassed;
            _playerCharModel.CellPositionChanged += OnCellPositionChanged;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<RequestPlayerCellChangeEvent>(OnRequestPlayerCellChanged);
            _eventBus.Unsubscribe<SpendMoneyOnBuildPointAnimationHalfEvent>(OnSpendMoneyOnBuildPointHalfAnimation);
            _eventBus.Unsubscribe<SpendMoneyOnBuildPointLastAnimationFinishedEvent>(OnSpendMoneyOnBuildPointLastAnimationFinished);
            _eventBus.Unsubscribe<TruckArrivedEvent>(OnTruckArrivedEvent);
            _eventBus.Unsubscribe<PutProductOnShelfHalfAnimationEvent>(OnPutProductOnShelfHalfAnimationEvent);
            _updatesProvider.SecondPassed -= OnSecondPassed;
            _updatesProvider.QuarterSecondPassed -= OnQuarterSecondPassed;
            _playerCharModel.CellPositionChanged -= OnCellPositionChanged;
        }

        private void OnSecondPassed()
        {
            PutProductOnShelfIfNeeded(_playerCharModel.CellPosition);
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
            CheckNearShopObjects(cellPosition);
        }

        private void CheckNearShopObjects(Vector2Int cellPosition)
        {
            CashDeskModel nearCashDesk = null;
            ShelfModel nearShelf = null;
            TruckPointModel nearTruckPoint = null;

            if (cellPosition.x==0 && 
                (_shopModel.TryGetTruckPoint(cellPosition + Vector2Int.left, out var truckPointModel)
                || _shopModel.TryGetTruckPoint(cellPosition + Vector2Int.left + Vector2Int.up, out truckPointModel)))
            {
                nearTruckPoint = truckPointModel;
            }
            else
            {
                foreach (var cellOffset in Constants.NearCells8)
                {
                    var nearCell = cellPosition + cellOffset;

                    if (_ownedCellsDataHolder.TryGetCashDesk(nearCell, out var cashDeskModel))
                    {
                        nearCashDesk = cashDeskModel;
                        break;
                    }

                    if (_ownedCellsDataHolder.TryGetShelf(nearCell, out var shelfModel))
                    {
                        nearShelf = shelfModel;
                        break;
                    }
                }
            }

            _playerCharModel.SetNearCashDesk(nearCashDesk);
            _playerCharModel.SetNearShelf(nearShelf);
            _playerCharModel.SetNearTruckPoint(nearTruckPoint);
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
                    new AnimatePutProductOnShelfEvent(nearEmptyShelf, _playerCharModel.ProductsBox, productToMove, boxSlotIndex, shelfSlotIndex));
            }
        }

        private void OnPutProductOnShelfHalfAnimationEvent(PutProductOnShelfHalfAnimationEvent e)
        {
            if (e.ProductBoxModel == _playerCharModel.ProductsBox)
            {
                _putProductsIsBlocked = false;
                PutProductOnShelfIfNeeded(_playerCharModel.CellPosition);
                
            }
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
                && (_shopModel.TryGetTruckPoint(cellPos - TruckPointHelper.PrimaryInteractionCellOffset, out var truckPointModel)
                    || _shopModel.TryGetTruckPoint(cellPos - TruckPointHelper.SecondaryInteractionCellOffset, out truckPointModel)))
            {
                if (truckPointModel.DeliverTimeSecondsRest <= 0
                    && truckPointModel.HasProducts
                    && _playerCharModel.HasProducts == false)
                {
                    var productBoxIndexToTake = truckPointModel.GetLastNotEmptyProductBoxIndex();
                    var productTypeToTake = truckPointModel.GetProductTypeAtBoxIndex(productBoxIndexToTake);
                    
                    truckPointModel.RemoveBox(productBoxIndexToTake);
                    _playerCharModel.FillBoxWithProduct(productTypeToTake);
                    
                    _eventBus.Dispatch(new AnimateTakeBoxFromTruckEvent(truckPointModel, productBoxIndexToTake));
                }
            }
        }

        private void TriggerSpendOnBuildPointIterationAnimationIfNeeded(Vector2Int cellPosition)
        {
            if (_shopModel.BuildPoints.TryGetValue(cellPosition, out var buildPoint)
                && buildPoint.MoneyToBuildLeft > 0
                && _playerModel.MoneyAmount > 0)
            {
                var deltaMoneyAmount = GetMoneyPerSingleAnimation(buildPoint.MoneyToBuildLeft);
                
                _playerModel.ChangeMoney(-deltaMoneyAmount);
                buildPoint.ChangeMoneyToBuildLeft(-deltaMoneyAmount);

                _eventBus.Dispatch(new TriggerSpendMoneyOnBuildPointAnimationEvent(
                    buildPoint,
                    buildPoint.MoneyToBuildLeft + deltaMoneyAmount,
                    buildPoint.MoneyToBuildLeft));
            }
        }

        private void OnSpendMoneyOnBuildPointHalfAnimation(SpendMoneyOnBuildPointAnimationHalfEvent e)
        {
            if (_shopModel.BuildPoints.TryGetValue(e.TargetBuildPointCellCoords, out var buildPoint))
            {
                if (buildPoint.MoneyToBuildLeft > 0)
                {
                    TriggerSpendOnBuildPointIterationAnimationIfNeeded(_playerCharModel.CellPosition);
                }
            }
        }

        private void OnSpendMoneyOnBuildPointLastAnimationFinished(SpendMoneyOnBuildPointLastAnimationFinishedEvent e)
        {
            if (_shopModel.BuildPoints.TryGetValue(e.TargetBuildPointCellCoords, out var buildPoint))
            {
                if (buildPoint.MoneyToBuildLeft <= 0)
                {
                    _commandExecutor.Execute<BuildShopObjectCommand, BuildPointModel>(buildPoint);
                }
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

            return Math.Min(result, _playerModel.MoneyAmount);
        }
    }
}