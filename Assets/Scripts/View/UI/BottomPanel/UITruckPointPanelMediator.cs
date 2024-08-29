using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.People;
using Model.ShopObjects;
using Utils;

namespace View.UI.BottomPanel
{
    public class UITruckPointPanelMediator : UIBottomPanelMediatorBase<UITruckPointPanelView>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly SpritesHolderSo _spritesHolder = Instance.Get<SpritesHolderSo>();
        private readonly IUpgradeCostProvider _upgradeCostProvider = Instance.Get<IUpgradeCostProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IHireStaffCostProvider _hireStaffCostProvider = Instance.Get<IHireStaffCostProvider>();
        
        private PlayerCharModel _playerCharModel;
        private TruckPointModel _targetTruckPoint;
        private string _secondsPostfix;

        protected override void MediateInternal()
        {
            base.MediateInternal();
            
            _playerCharModel = _playerModelHolder.PlayerCharModel;

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerCharModel.NearShopObjectsUpdated += OnNearShopObjectsUpdated;
            PanelView.UpgradeButtonClicked += OnUpgradeButtonClicked;
            PanelView.HireStaffButtonClicked += OnHireStaffButtonClicked;
            _updatesProvider.SecondPassed += OnSecondPassed;
        }

        private void Unsubscribe()
        {
            _playerCharModel.NearShopObjectsUpdated -= OnNearShopObjectsUpdated;
            PanelView.UpgradeButtonClicked -= OnUpgradeButtonClicked;
            PanelView.HireStaffButtonClicked -= OnHireStaffButtonClicked;
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

        private void OnNearShopObjectsUpdated()
        {
            if (_playerCharModel.NearTruckPoint != null 
                && _playerCharModel.IsMultipleShopObjectsNear == false
                && _playerModelHolder.PlayerModel.Level >= Constants.MinLevelForTruckPointUpgrades)
            {
                _secondsPostfix = _localizationProvider.GetLocale(Constants.LocalizationSecondsShortPostfix);
                
                ProcessNewTargetTruckPoint(_playerCharModel.NearTruckPoint);
                
                SlideUp();
            }
            else
            {
                ResetTargetTruckPoint();
                    
                SlideDown();
            }
        }

        private void DisplayProductIcons(TruckPointModel targetTruckPoint)
        {
            var availableProducts = targetTruckPoint.GetAvailableProducts();
            
            for (var i = 0; i < PanelView.ProductIconsAmount; i++)
            {
                var productType = i < availableProducts.Length ? availableProducts[i] : ProductType.None;
                var productSprite = _spritesHolder.GetProductSpriteByKey(productType);
                
                PanelView.SetProductIconSprite(i, productSprite);
            }
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

            PanelView.SetDeliverTitleText($"{deliveryText} ({deliverTimeSeconds}{_secondsPostfix})");
            PanelView.SetStaffTitleText(_localizationProvider.GetLocale(Constants.LocalizationBottomPanelStaffTitle));
        }

        private void OnTruckPointUpgraded()
        {
            UpdateUpgradeBlockView();

            _eventBus.Dispatch(
                new UIRequestFlyingTextEvent(
                    _localizationProvider.GetLocale(Constants.LocalizationUpgraded),
                    PanelView.UpgradeButtonTransform.position));
        }

        private void OnStaffAdded(TruckPointStaffCharModel __)
        {
            UpdateHireBlockView();
            
            _eventBus.Dispatch(
                new UIRequestFlyingTextEvent(
                    _localizationProvider.GetLocale(Constants.LocalizationHired),
                    PanelView.HireStaffButtonView.transform.position));
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

                    PanelView.SetUpgradeButtonText(upgradeText);
                    PanelView.SetUpgradeEnabledState(true);

                    return;
                }
            }

            PanelView.SetUpgradeButtonText(_localizationProvider.GetLocale(Constants.LocalizationMaxUpgrade));
            PanelView.SetUpgradeEnabledState(false);
        }

        private void DisplayStaff(TruckPointModel truckPointModel)
        {
            for (var i = 0; i < truckPointModel.StaffCharModels.Count; i++)
            {
                var staffModel = truckPointModel.StaffCharModels[i];
                var isStaffExists = staffModel != null;
                PanelView.SetStaffEnabled(i, isStaffExists);
                if (staffModel != null)
                {
                    PanelView.SetStaffWorkTimerText(i, $"{staffModel.WorkSecondsLeft}{_secondsPostfix}");
                    PanelView.SetStaffWorkTimeProgress(i, (float)staffModel.WorkSecondsLeft / staffModel.WorkSecondsSetting);
                }
            }
        }

        private void SetupHireButton(TruckPointModel truckPointModel)
        {
            var hireStaffCost = _hireStaffCostProvider.GetTruckPointHireStaffCost(truckPointModel);

            PanelView.SetHireStaffButtonInteractable(hireStaffCost >= 0);
            
            switch (hireStaffCost)
            {
                case > 0:
                {
                    var hireText =
                        $"{_localizationProvider.GetLocale(Constants.LocalizationHireButton)}\n{FormattingHelper.ToMoneyWithIconTextFormat(hireStaffCost)}";
                    PanelView.SetHireStaffButtonText(hireText);
                
                    PanelView.HireStaffButtonView.SetOrangeSkinData();
                    break;
                }
                case 0:
                {
                    var hireText =
                        $"{Constants.TextIconAds}\n{_localizationProvider.GetLocale(Constants.LocalizationHireButton)}";
                    PanelView.SetHireStaffButtonText(hireText);
                
                    PanelView.HireStaffButtonView.SetCrimsonSkinData();
                    break;
                }
                default:
                    PanelView.SetHireStaffButtonText(_localizationProvider.GetLocale(Constants.LocalizationHireButton));
                    PanelView.HireStaffButtonView.SetOrangeSkinData();
                    break;
            }
        }
    }
}