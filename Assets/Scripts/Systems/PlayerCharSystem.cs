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
using UnityEngine;

namespace Systems
{
    public class PlayerCharSystem : ISystem
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ICommandExecutor _commandExecutor = Instance.Get<ICommandExecutor>();
        
        private ShopModel _shopModel;
        private PlayerModel _playerModel;
        private PlayerCharModel _playerCharModel;

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
            _playerCharModel.CellPositionChanged += OnCellPositionChanged;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<RequestPlayerCellChangeEvent>(OnRequestPlayerCellChanged);
            _eventBus.Unsubscribe<SpendMoneyOnBuildPointAnimationHalfEvent>(OnSpendMoneyOnBuildPointHalfAnimation);
            _eventBus.Unsubscribe<SpendMoneyOnBuildPointAnimationFinishedEvent>(OnSpendMoneyOnBuildPointAnimationFinished);
            _eventBus.Unsubscribe<TruckArrivedEvent>(OnTruckArrivedEvent);
            _playerCharModel.CellPositionChanged -= OnCellPositionChanged;
        }

        private void OnRequestPlayerCellChanged(RequestPlayerCellChangeEvent e)
        {
            _playerCharModel.SetCellPosition(e.CellCoords);
        }

        private void OnCellPositionChanged(Vector2Int cellPosition)
        {
            TriggerSpendOnBuildPointIterationAnimationIfNeeded(cellPosition);
            TakeTruckProductBoxIfNeeded(cellPosition);
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
                    var productsToAdd = Enumerable.Repeat(productTypeToTake, Constants.ProductsAmountInBox);
                    
                    truckPointModel.RemoveBox(productBoxIndexToTake);
                    _playerCharModel.AddProductsBox(productsToAdd);
                    
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
                > 500 => 50,
                > 100 => 20,
                > 50 => 10,
                > 10 => 3,
                > 5 => 2,
                _ => 1
            };

            return Math.Min(result, _playerModel.Money);
        }
    }
}