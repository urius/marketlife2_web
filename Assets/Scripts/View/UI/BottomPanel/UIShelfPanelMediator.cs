using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using Utils;

namespace View.UI.BottomPanel
{
    public class UIShelfPanelMediator : UIBottomPanelMediatorBase<UIShelfPanelView>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ILocalizationProvider _loc = Instance.Get<ILocalizationProvider>();
        private readonly IUpgradeCostProvider _upgradeCostProvider = Instance.Get<IUpgradeCostProvider>();
        private readonly IShelfUpgradeSettingsProvider _shelfUpgradeSettingsProvider = Instance.Get<IShelfUpgradeSettingsProvider>();
        
        private PlayerCharModel _playerCharModel;
        private ShelfModel _targetShelfModel;
        
        protected override void MediateInternal()
        {
            base.MediateInternal();
            
            _playerCharModel = _playerModelHolder.PlayerCharModel;
            
            PanelView.UpgradeButton.SetOrangeSkinData();

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
        }

        private void Unsubscribe()
        {
            _playerCharModel.NearShopObjectsUpdated -= OnNearShopObjectsUpdated;
            PanelView.UpgradeButtonClicked -= OnUpgradeButtonClicked;
            
            UnsubscribeFromTargetShelf();
        }

        private void OnNearShopObjectsUpdated()
        {
            if (_playerCharModel.NearShelf != null 
                && _playerCharModel.IsMultipleShopObjectsNear == false
                && _playerModelHolder.PlayerModel.Level >= Constants.MinLevelForShelfUpgrades)
            {
                ProcessNewTargetShelfModel(_playerCharModel.NearShelf);

                SlideUp();
            }
            else
            {
                ResetTargetShelfModel();

                SlideDown();
            }
        }

        private void ProcessNewTargetShelfModel(ShelfModel shelfModel)
        {
            ResetTargetShelfModel();
            
            _targetShelfModel = shelfModel;
            
            var canUpgrade = CanUpgradeShelfModel(shelfModel);

            UpdateView(canUpgrade, shelfModel);

            SubscribeOnTargetShelf();
        }

        private void SubscribeOnTargetShelf()
        {
            if (_targetShelfModel == null) return;

            UnsubscribeFromTargetShelf();
            _targetShelfModel.UpgradeIndexChanged += OnUpgradeIndexChanged;
        }

        private void ResetTargetShelfModel()
        {
            UnsubscribeFromTargetShelf();
            _targetShelfModel = null;
        }

        private void UnsubscribeFromTargetShelf()
        {
            if (_targetShelfModel == null) return;

            _targetShelfModel.UpgradeIndexChanged -= OnUpgradeIndexChanged;
        }

        private void OnUpgradeIndexChanged(int upgradeIndex)
        {
            var canUpgrade = CanUpgradeShelfModel(_targetShelfModel);
            UpdateView(canUpgrade, _targetShelfModel);

            var upgradedText = _loc.GetLocale(Constants.LocalizationUpgraded);
            _eventBus.Dispatch(new UIRequestFlyingTextEvent(upgradedText, PanelView.UpgradeButton.transform.position));
        }

        private void OnUpgradeButtonClicked()
        {
            if (_targetShelfModel == null) return;
            
            _eventBus.Dispatch(new UIShelfUpgradeClickedEvent(_targetShelfModel));
        }

        private void UpdateView(bool canUpgrade, ShelfModel shelfModel)
        {
            var titleLocale = _loc.GetLocale(Constants.LocalizationUpgradeShelf);
            PanelView.SetTitleText(titleLocale);

            if (canUpgrade)
            {            
                var upgradeCost = _upgradeCostProvider.GetShelfUpgradeCost(shelfModel);
                var upgradeText =
                    $"{_loc.GetLocale(Constants.LocalizationUpgradeButton)}\n{FormattingHelper.ToMoneyWithIconTextFormat(upgradeCost)}";
                
                PanelView.UpgradeButton.SetText(upgradeText);
                PanelView.UpgradeButton.SetInteractable(true);
            }
            else
            {
                var maxUpgradeLocale = _loc.GetLocale(Constants.LocalizationMaxUpgrade);
                
                PanelView.UpgradeButton.SetText(maxUpgradeLocale);
                PanelView.UpgradeButton.SetInteractable(false);
            }
        }

        private bool CanUpgradeShelfModel(ShelfModel shelfModel)
        {
            return shelfModel != null
                   && _shelfUpgradeSettingsProvider.CanUpgradeTo(
                       shelfModel.ShopObjectType, shelfModel.UpgradeIndex + 1);
        } 
    }
}