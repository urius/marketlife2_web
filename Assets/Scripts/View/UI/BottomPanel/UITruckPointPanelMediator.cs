using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.People;
using Model.ShopObjects;
using Utils;
using View.Game.People;
using View.Helpers;

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
        private readonly IPlayerFocusProvider _playerFocusProvider = Instance.Get<IPlayerFocusProvider>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();

        private PlayerModel _playerModel;
        private PlayerCharModel _playerCharModel;
        private TruckPointModel _targetTruckPoint;
        private string _secondsPostfix;
        private int _workSecondsLeftTemp;

        protected override void MediateInternal()
        {
            base.MediateInternal();

            _sharedViewsDataHolder.RegisterTruckPointPanelTransformsProvider(PanelView);
            
            _playerModel = _playerModelHolder.PlayerModel;
            _playerCharModel = _playerModelHolder.PlayerCharModel;

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            _sharedViewsDataHolder.UnregisterTruckPointPanelTransformsProvider();
            
            Unsubscribe();
        }

        private void Subscribe()
        {
            SlideUpFinished += OnSlideUpFinished;
            SlideDownFinished += OnSlideDownFinished;
            
            _playerCharModel.NearShopObjectsUpdated += OnNearShopObjectsUpdated;
            PanelView.UpgradeButtonClicked += OnUpgradeButtonClicked;
            PanelView.HireStaffButtonClicked += OnHireStaffButtonClicked;
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
            _playerFocusProvider.PlayerFocusChanged += OnPlayerFocusChanged;
        }

        private void Unsubscribe()
        {
            SlideUpFinished -= OnSlideUpFinished;
            SlideDownFinished -= OnSlideDownFinished;
            
            _playerCharModel.NearShopObjectsUpdated -= OnNearShopObjectsUpdated;
            PanelView.UpgradeButtonClicked -= OnUpgradeButtonClicked;
            PanelView.HireStaffButtonClicked -= OnHireStaffButtonClicked;
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            _playerFocusProvider.PlayerFocusChanged -= OnPlayerFocusChanged;
            
            UnsubscribeFromTruckPoint(_targetTruckPoint);
        }

        private void OnSlideUpFinished()
        {
            _eventBus.Dispatch(new UITruckPointBottomPanelSlideAnimationFinishedEvent(isSlideUp: true));
        }

        private void OnSlideDownFinished()
        {            
            _eventBus.Dispatch(new UITruckPointBottomPanelSlideAnimationFinishedEvent(isSlideUp: false));
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
            UpdatePanelViewState();
        }

        private void OnPlayerFocusChanged(bool isFocused)
        {
            UpdatePanelViewState();
        }

        private void UpdatePanelViewState()
        {
            if (_playerCharModel.NearTruckPoint != null
                && _playerCharModel.IsMultipleShopObjectsNear == false
                && _playerModelHolder.PlayerModel.Level >= Constants.MinLevelForTruckPointUpgrades
                && _playerFocusProvider.IsPlayerFocused)
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
            ResetTargetTruckPoint();
            
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
            
            RequestFlyingText(Constants.LocalizationHired, PanelView.HireStaffButtonView.transform);
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

        private void OnGameplayFixedUpdate()
        {
            if (_targetTruckPoint == null) return;
            
            var workSecondsLeft = _targetTruckPoint.HasStaff ? _targetTruckPoint.StaffCharModel.WorkSecondsLeft : -1;
            if (workSecondsLeft == _workSecondsLeftTemp) return;
            
            if (_workSecondsLeftTemp > 0 && workSecondsLeft > _workSecondsLeftTemp)
            {
                RequestFlyingText(Constants.LocalizationProlonged, PanelView.HireStaffButtonView.transform);
            }

            _workSecondsLeftTemp = workSecondsLeft;
            UpdateHireBlockView();
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
            var staffModel = truckPointModel.StaffCharModel;
            
            PanelView.SetStaffEnabled(truckPointModel.HasStaff);
            
            if (staffModel != null)
            {
                PanelView.SetStaffWorkTimerText($"{staffModel.WorkSecondsLeft}{_secondsPostfix}");
                
                var clockColor = StaffCharHelper.GetClockColorByPercent(
                        (float)staffModel.WorkSecondsLeft /
                        _playerModel.StaffWorkTimeSeconds);
                    
                    PanelView.SetClockColor(clockColor);
            }
        }

        private void SetupHireButton(TruckPointModel truckPointModel)
        {
            var hireStaffCost = _hireStaffCostProvider.GetTruckPointHireStaffCost(truckPointModel);
            
            ButtonsHelper.SetupHireButtonText(PanelView.HireStaffButtonView, truckPointModel.StaffCharModel, hireStaffCost);
        }
    }
}