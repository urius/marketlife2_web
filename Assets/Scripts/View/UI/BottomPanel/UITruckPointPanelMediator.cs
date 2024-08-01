using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.People;
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
        private readonly IHireStaffCostProvider _hireStaffCostProvider = Instance.Get<IHireStaffCostProvider>();
        
        private PlayerCharModel _playerCharModel;
        private UITruckPointPanelView _truckPointPanelView;
        private float _slideUpPositionPercent = 0;
        private int _slideDirection = 1;
        private TruckPointModel _targetTruckPoint;
        private string _secondsPostfix;

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
            _truckPointPanelView.HireStaffButtonClicked += OnHireStaffButtonClicked;
            _updatesProvider.SecondPassed += OnSecondPassed;
        }

        private void Unsubscribe()
        {
            _playerCharModel.NearTruckPointUpdated -= OnNearTruckPointUpdated;
            _truckPointPanelView.UpgradeButtonClicked -= OnUpgradeButtonClicked;
            _truckPointPanelView.HireStaffButtonClicked -= OnHireStaffButtonClicked;
            _updatesProvider.SecondPassed -= OnSecondPassed;
            
            UnsubscribeFromTruckPoint(_targetTruckPoint);
        }

        private void OnUpgradeButtonClicked()
        {
            if (_targetTruckPoint != null)
            {
                _eventBus.Dispatch(new UpgradeTruckPointButtonClickedEvent(_targetTruckPoint));
            }
        }

        private void OnHireStaffButtonClicked()
        {
            if (_targetTruckPoint != null)
            {
                _eventBus.Dispatch(new TruckPointHireStaffButtonClickedEvent(_targetTruckPoint));
            }
        }

        private void OnNearTruckPointUpdated()
        {
            if (_playerCharModel.NearTruckPoint != null)
            {
                _secondsPostfix = _localizationProvider.GetLocale(Constants.LocalizationSecondsShortPostfix);
                
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
            _updatesProvider.GameplayFixedUpdate -= OnSlideGameplayFixedUpdate;
            _updatesProvider.GameplayFixedUpdate += OnSlideGameplayFixedUpdate;
        }

        private void OnSlideGameplayFixedUpdate()
        {
            _slideUpPositionPercent += 5 * Time.fixedDeltaTime * _slideDirection;

            if (_slideUpPositionPercent >= 1)
            {
                _slideUpPositionPercent = 1;
                _updatesProvider.GameplayFixedUpdate -= OnSlideGameplayFixedUpdate;
            }

            if (_slideUpPositionPercent <= 0)
            {
                ResetTargetTruckPoint();
                
                _slideUpPositionPercent = 0;
                _truckPointPanelView.SetActive(false);
                _updatesProvider.GameplayFixedUpdate -= OnSlideGameplayFixedUpdate;
            }
            
            _truckPointPanelView.SetSlideUpPositionPercent(_slideUpPositionPercent);
        }
        
        private void ProcessNewTargetTruckPoint(TruckPointModel truckPointModel)
        {
            _targetTruckPoint = truckPointModel;
            
            UpdateUpgradeBlockView();
            UpdateHireBlockView();

            SubscribeOnTruckPoint(_targetTruckPoint);
        }

        private void UpdateUpgradeBlockView()
        {
            DisplayTitles(_targetTruckPoint);
            DisplayProductIcons(_targetTruckPoint);
            SetupUpgradeButton(_targetTruckPoint);
        }

        private void ResetTargetTruckPoint()
        {
            UnsubscribeFromTruckPoint(_targetTruckPoint);
            _targetTruckPoint = null;
        }

        private void SubscribeOnTruckPoint(TruckPointModel targetTruckPoint)
        {
            targetTruckPoint.Upgraded += OnTruckPointUpgraded;
            targetTruckPoint.StaffAdded += OnStaffAdded;
            targetTruckPoint.StaffRemoved += OnStaffRemoved;
        }

        private void UnsubscribeFromTruckPoint(TruckPointModel targetTruckPoint)
        {
            if (targetTruckPoint == null) return;
            
            targetTruckPoint.Upgraded -= OnTruckPointUpgraded;
            targetTruckPoint.StaffAdded -= OnStaffAdded;
            targetTruckPoint.StaffRemoved -= OnStaffRemoved;
        }

        private void DisplayTitles(TruckPointModel targetTruckPoint)
        {
            var deliverTimeSeconds = targetTruckPoint.GetDeliverTimeSettingSeconds();

            var deliveryText = _localizationProvider.GetLocale(Constants.LocalizationBottomPanelDeliveryTitle);

            _truckPointPanelView.SetDeliverTitleText($"{deliveryText} ({deliverTimeSeconds}{_secondsPostfix})");
            _truckPointPanelView.SetStaffTitleText(_localizationProvider.GetLocale(Constants.LocalizationBottomPanelStaffTitle));
        }

        private void OnTruckPointUpgraded()
        {
            UpdateUpgradeBlockView();

            _eventBus.Dispatch(
                new UIRequestFlyingTextEvent(
                    _localizationProvider.GetLocale(Constants.LocalizationUpgraded),
                    _truckPointPanelView.UpgradeButtonTransform.position));
        }

        private void OnStaffAdded(TruckPointStaffCharModel __)
        {
            UpdateHireBlockView();
            
            _eventBus.Dispatch(
                new UIRequestFlyingTextEvent(
                    _localizationProvider.GetLocale(Constants.LocalizationHired),
                    _truckPointPanelView.HireStaffButtonView.transform.position));
        }

        private void OnStaffRemoved(TruckPointStaffCharModel __)
        {
            UpdateHireBlockView();
        }

        private void UpdateHireBlockView()
        {
            DisplayStaff(_targetTruckPoint);
            SetupHireButton(_targetTruckPoint);
        }

        private void OnSecondPassed()
        {
            if (_targetTruckPoint != null)
            {
                UpdateHireBlockView();
            }
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

        private void DisplayStaff(TruckPointModel truckPointModel)
        {
            for (var i = 0; i < truckPointModel.StaffCharModels.Count; i++)
            {
                var staffModel = truckPointModel.StaffCharModels[i];
                var isStaffExists = staffModel != null;
                _truckPointPanelView.SetStaffEnabled(i, isStaffExists);
                if (staffModel != null)
                {
                    _truckPointPanelView.SetStaffWorkTimerText(i, $"{staffModel.WorkSecondsLeft}{_secondsPostfix}");
                    _truckPointPanelView.SetStaffWorkTimeProgress(i, (float)staffModel.WorkSecondsLeft / staffModel.WorkSecondsSetting);
                }
            }
        }

        private void SetupHireButton(TruckPointModel truckPointModel)
        {
            var hireStaffCost = _hireStaffCostProvider.GetTruckPointHireStaffCost(truckPointModel);

            _truckPointPanelView.SetHireStaffButtonInteractable(hireStaffCost >= 0);
            
            switch (hireStaffCost)
            {
                case > 0:
                {
                    var hireText =
                        $"{_localizationProvider.GetLocale(Constants.LocalizationHireButton)}\n{FormattingHelper.ToMoneyWithIconTextFormat(hireStaffCost)}";
                    _truckPointPanelView.SetHireStaffButtonText(hireText);
                
                    _truckPointPanelView.HireStaffButtonView.SetOrangeSkinData();
                    break;
                }
                case 0:
                {
                    var hireText =
                        $"{Constants.TextIconAds}\n{_localizationProvider.GetLocale(Constants.LocalizationHireButton)}";
                    _truckPointPanelView.SetHireStaffButtonText(hireText);
                
                    _truckPointPanelView.HireStaffButtonView.SetCrimsonSkinData();
                    break;
                }
                default:
                    _truckPointPanelView.SetHireStaffButtonText(_localizationProvider.GetLocale(Constants.LocalizationHireButton));
                    _truckPointPanelView.HireStaffButtonView.SetOrangeSkinData();
                    break;
            }
        }
    }
}