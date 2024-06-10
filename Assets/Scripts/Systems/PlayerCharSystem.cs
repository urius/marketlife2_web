using System;
using Data;
using Events;
using Holders;
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
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IShelfSettingsProvider _shelfSettingsProvider = Instance.Get<IShelfSettingsProvider>();
        
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
            _playerCharModel.CellPositionChanged += OnCellPositionChanged;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<RequestPlayerCellChangeEvent>(OnRequestPlayerCellChanged);
            _eventBus.Unsubscribe<SpendMoneyOnBuildPointAnimationHalfEvent>(OnSpendMoneyOnBuildPointHalfAnimation);
            _eventBus.Unsubscribe<SpendMoneyOnBuildPointAnimationFinishedEvent>(OnSpendMoneyOnBuildPointAnimationFinished);
            _playerCharModel.CellPositionChanged -= OnCellPositionChanged;
        }

        private void OnRequestPlayerCellChanged(RequestPlayerCellChangeEvent e)
        {
            _playerCharModel.SetCellPosition(e.CellCoords);
        }

        private void OnCellPositionChanged(Vector2Int cellPosition)
        {
            TriggerSpendOnBuildPointIterationAnimationIfNeeded(cellPosition);
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
                    _shopModel.RemoveBuildPoint(buildPoint.CellCoords);
                    
                    var shopObject = GetShopObjectByBuildPoint(buildPoint);
                    _shopModel.AddShopObject(shopObject);
                }
            }
        }

        private ShopObjectModelBase GetShopObjectByBuildPoint(BuildPointModel buildPoint)
        {
            ShopObjectModelBase result;
            
            var shopObjectType = buildPoint.ShopObjectType;
            var pointOffset = GetBuildPointOffset(shopObjectType);
            var buildCoords = buildPoint.CellCoords + pointOffset;
            
            switch (shopObjectType)
            {
                case ShopObjectType.CashDesk:
                    result = new CashDeskModel(buildCoords);
                    break;
                default:
                    if (shopObjectType.IsShelf())
                    {
                        _shelfSettingsProvider.TryGetShelfSetting(shopObjectType, 0, out var shelfSettings);
                        
                        result = new ShelfModel(buildCoords, shopObjectType, shelfSettings.SlotsAmount);
                    }
                    else
                    {
                        throw new NotImplementedException(
                            $"{nameof(GetShopObjectByBuildPoint)}: unknown shopObjectType {shopObjectType}");
                    }
                    break;
            }

            return result;
        }

        private Vector2Int GetBuildPointOffset(ShopObjectType shopObjectType)
        {
            var result = Vector2Int.zero;
            
            switch (shopObjectType)
            {
                case ShopObjectType.CashDesk:
                    result = Vector2Int.left;
                    break;
                default:
                    if (shopObjectType.IsShelf())
                    {
                        result = Vector2Int.left;
                    }
                    else
                    {
                        Debug.LogError($"{nameof(GetBuildPointOffset)}: Unknown shopObjectType: {shopObjectType}");
                    }
                    break;
            }

            return result;
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