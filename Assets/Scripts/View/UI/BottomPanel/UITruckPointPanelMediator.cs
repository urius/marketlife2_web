using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using UnityEngine;
using Utils;

namespace View.UI.BottomPanel
{
    public class UITruckPointPanelMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly SpritesHolderSo _spritesHolder = Instance.Get<SpritesHolderSo>();
        private readonly IUpgradeCostProvider _upgradeCostProvider = Instance.Get<IUpgradeCostProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private PlayerCharModel _playerCharModel;
        private UITruckPointPanelView _truckPointPanelView;
        private float _slideUpPositionPercent = 0;
        private int _slideDirection = 1;
        private TruckPointModel _targetTruckPoint;

        protected override void MediateInternal()
        {
            _playerCharModel = _playerModelHolder.PlayerCharModel;
            _truckPointPanelView = TargetTransform.GetComponent<UITruckPointPanelView>();

            _truckPointPanelView.SetSlideUpPositionPercent(_slideUpPositionPercent);
            _truckPointPanelView.SetActive(false);

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerCharModel.NearTruckPointUpdated += OnNearTruckPointUpdated;
            _truckPointPanelView.UpgradeButtonClicked += OnUpgradeButtonClicked;
        }

        private void Unsubscribe()
        {
            _playerCharModel.NearTruckPointUpdated -= OnNearTruckPointUpdated;
            _truckPointPanelView.UpgradeButtonClicked -= OnUpgradeButtonClicked;
            
            UnsubscribeFromTruckPoint(_targetTruckPoint);
        }

        private void OnUpgradeButtonClicked()
        {
            if (_targetTruckPoint != null)
            {
                _eventBus.Dispatch(new UpgradeTruckPointButtonClickedEvent(_targetTruckPoint));
            }
        }

        private void OnNearTruckPointUpdated()
        {
            if (_playerCharModel.NearTruckPoint != null)
            {
                ProcessNewTargetTruckPoint(_playerCharModel.NearTruckPoint);
                
                _truckPointPanelView.SetActive(true);
                _slideDirection = 1;
            }
            else
            {
                _slideDirection = -1;
            }
            
            ResubscribeOnUpdate();
        }

        private void DisplayProductIcons(TruckPointModel targetTruckPoint)
        {
            var availableProducts = targetTruckPoint.GetAvailableProducts();
            
            for (var i = 0; i < _truckPointPanelView.ProductIconsAmount; i++)
            {
                var productType = i < availableProducts.Length ? availableProducts[i] : ProductType.None;
                var productSprite = _spritesHolder.GetProductSpriteByKey(productType);
                
                _truckPointPanelView.SetProductIconSprite(i, productSprite);
            }
        }
        
        private void ResubscribeOnUpdate()
        {
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
        }

        private void OnGameplayFixedUpdate()
        {
            _slideUpPositionPercent += 5 * Time.fixedDeltaTime * _slideDirection;

            if (_slideUpPositionPercent >= 1)
            {
                _slideUpPositionPercent = 1;
                _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            }

            if (_slideUpPositionPercent <= 0)
            {
                ResetTargetTruckPoint();
                
                _slideUpPositionPercent = 0;
                _truckPointPanelView.SetActive(false);
                _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            }
            
            _truckPointPanelView.SetSlideUpPositionPercent(_slideUpPositionPercent);
        }


        private void ProcessNewTargetTruckPoint(TruckPointModel truckPointModel)
        {
            _targetTruckPoint = truckPointModel;
            
            DisplayTitles(_targetTruckPoint);
            DisplayProductIcons(_targetTruckPoint);
            SetupUpgradeButton(_targetTruckPoint);

            SubscribeOnTruckPoint(_targetTruckPoint);
        }

        private void ResetTargetTruckPoint()
        {
            UnsubscribeFromTruckPoint(_targetTruckPoint);
            _targetTruckPoint = null;
        }

        private void SubscribeOnTruckPoint(TruckPointModel targetTruckPoint)
        {
            targetTruckPoint.Upgraded += OnTruckPointUpgraded;
        }

        private void UnsubscribeFromTruckPoint(TruckPointModel targetTruckPoint)
        {
            if (targetTruckPoint == null) return;
            
            targetTruckPoint.Upgraded -= OnTruckPointUpgraded;
        }

        private void DisplayTitles(TruckPointModel targetTruckPoint)
        {
            var deliverTimeSeconds = targetTruckPoint.GetDeliverTimeSettingSeconds();

            var deliveryText = _localizationProvider.GetLocale(Constants.LocalizationBottomPanelDeliveryTitle);
            var secondsPostfix = _localizationProvider.GetLocale(Constants.LocalizationSecondsShortPostfix);
            
            _truckPointPanelView.SetDeliverTitleText(
                $"{deliveryText} ({deliverTimeSeconds}{secondsPostfix})");
            _truckPointPanelView.SetStaffTitleText(_localizationProvider.GetLocale(Constants.LocalizationBottomPanelStaffTitle));
        }

        private void OnTruckPointUpgraded()
        {
            DisplayTitles(_targetTruckPoint);
            DisplayProductIcons(_targetTruckPoint);
            SetupUpgradeButton(_targetTruckPoint);
        }

        private void SetupUpgradeButton(TruckPointModel targetTruckPoint)
        {
            if (targetTruckPoint.CanUpgrade())
            {
                var upgradeCost = _upgradeCostProvider.GetTruckPointUpgradeCost(targetTruckPoint);
                if (upgradeCost > 0)
                {
                    var upgradeText =
                        $"{_localizationProvider.GetLocale(Constants.LocalizationUpgradeButton)}\n{FormattingHelper.ToMoneyWithIconTextFormat(upgradeCost)}";

                    _truckPointPanelView.SetUpgradeButtonText(upgradeText);
                    _truckPointPanelView.SetUpgradeEnabledState(true);

                    return;
                }
            }

            _truckPointPanelView.SetUpgradeButtonText(_localizationProvider.GetLocale(Constants.LocalizationMaxUpgrade));
            _truckPointPanelView.SetUpgradeEnabledState(false);
        }
    }
}