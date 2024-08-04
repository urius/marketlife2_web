using System;
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
            _eventBus.Subscribe<SpendMoneyOnBuildPointLastAnimationFinishedEvent>(OnSpendMoneyOnBuildPointLastAnimationFinished);
            _eventBus.Subscribe<TruckArrivedEvent>(OnTruckArrivedEvent);
            _eventBus.Subscribe<PutProductOnShelfHalfAnimationEvent>(OnPutProductOnShelfHalfAnimationEvent);
            _updatesProvider.SecondPassed += OnSecondPassed;
            _updatesProvider.QuarterSecondPassed += OnQuarterSecondPassed;
            _playerCharModel.CellPositionChanged += OnCellPositionChanged;
            _shopModel.ShopObjectAdded += OnShopObjectAdded;
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
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
        }

        private void OnSecondPassed()
        {
            PutProductOnShelfIfNeeded();
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
            CheckNearShopObjects(cellPosition);
            
            TriggerSpendOnBuildPointIterationAnimationIfNeeded(cellPosition);
            TakeTruckProductBoxIfNeeded();
            PutProductOnShelfIfNeeded();
        }

        private void OnShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            CheckNearShopObjects(_playerCharModel.CellPosition);
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

        private void PutProductOnShelfIfNeeded()
        {
            if (!_playerCharModel.HasProducts
                || _putProductsIsBlocked
                || _playerCharModel.NearShelf == null
                || _playerCharModel.NearShelf.HasEmptySlots() == false) return;

            _putProductsIsBlocked = true;
            var nearEmptyShelf = _playerCharModel.NearShelf;

            var shelfSlotIndex = nearEmptyShelf.GetEmptySlotIndex();

            var boxSlotIndex = _playerCharModel.GetNextNotEmptySlotIndex();
            var productToMove = _playerCharModel.ProductsInBox[boxSlotIndex];

            _playerCharModel.RemoveProductFromSlot(boxSlotIndex);

            nearEmptyShelf.AddProductToSlot(shelfSlotIndex, productToMove);

            _eventBus.Dispatch(
                new AnimatePutProductOnShelfEvent(nearEmptyShelf, _playerCharModel.ProductsBox, productToMove,
                    boxSlotIndex, shelfSlotIndex));
        }

        private void OnPutProductOnShelfHalfAnimationEvent(PutProductOnShelfHalfAnimationEvent e)
        {
            if (e.ProductBoxModel == _playerCharModel.ProductsBox)
            {
                _putProductsIsBlocked = false;
                PutProductOnShelfIfNeeded();
            }
        }

        private void OnTruckArrivedEvent(TruckArrivedEvent e)
        {
            TakeTruckProductBoxIfNeeded();
        }

        private void TakeTruckProductBoxIfNeeded()
        {
            if (_playerCharModel.NearTruckPoint != null)
            {
                var truckPointModel = _playerCharModel.NearTruckPoint;
                
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