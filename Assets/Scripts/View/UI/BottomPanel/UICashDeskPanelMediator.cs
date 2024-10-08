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
    public class UICashDeskPanelMediator : UIBottomPanelMediatorBase<UICashDeskPanelView>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IHireStaffCostProvider _hireStaffCostProvider = Instance.Get<IHireStaffCostProvider>();
        private readonly IPlayerFocusProvider _playerFocusProvider = Instance.Get<IPlayerFocusProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        
        private PlayerCharModel _playerCharModel;
        private CashDeskModel _targetCashDeskModel;
        private int _workSecondsLeftTemp;
        private PlayerModel _playerModel;

        protected override void MediateInternal()
        {
            base.MediateInternal();
            
            _playerModel = _playerModelHolder.PlayerModel;
            _playerCharModel = _playerModelHolder.PlayerCharModel;
            
            _sharedViewsDataHolder.RegisterCashDeskPanelTransformsProvider(PanelView);
            
            UpdatePanelViewState();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();

            _sharedViewsDataHolder.UnregisterCashDeskPanelTransformsProvider();
        }

        private void Subscribe()
        {
            SlideUpFinished += OnSlideUpFinished;
            SlideDownFinished += OnSlideDownFinished;
            
            _playerModel.LevelChanged += OnLevelChanged;
            _playerCharModel.NearShopObjectsUpdated += OnNearShopObjectsUpdated;
            PanelView.HireStaffButtonClicked += OnHireStaffButtonClicked;
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
            _playerFocusProvider.PlayerFocusChanged += OnPlayerFocusChanged;
        }

        private void Unsubscribe()
        {
            SlideUpFinished -= OnSlideUpFinished;
            SlideDownFinished -= OnSlideDownFinished;
            
            _playerModel.LevelChanged -= OnLevelChanged;
            _playerCharModel.NearShopObjectsUpdated -= OnNearShopObjectsUpdated;
            PanelView.HireStaffButtonClicked -= OnHireStaffButtonClicked;
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            _playerFocusProvider.PlayerFocusChanged -= OnPlayerFocusChanged;
        }

        private void OnSlideUpFinished()
        {
            _eventBus.Dispatch(new UICashDeskBottomPanelSlideAnimationFinishedEvent(isSlideUp: true));
        }

        private void OnSlideDownFinished()
        {
            _eventBus.Dispatch(new UICashDeskBottomPanelSlideAnimationFinishedEvent(isSlideUp: false));
        }

        private void OnGameplayFixedUpdate()
        {
            if (_targetCashDeskModel == null) return;
            
            var workSecondsLeft = _targetCashDeskModel.HasCashMan ? _targetCashDeskModel.CashDeskStaffModel.WorkSecondsLeft : -1;
            if (workSecondsLeft == _workSecondsLeftTemp) return;

            if (_workSecondsLeftTemp > 0 && workSecondsLeft > _workSecondsLeftTemp)
            {
                RequestFlyingText(Constants.LocalizationProlonged, PanelView.HireStaffButtonView.transform);
            }
            
            _workSecondsLeftTemp = workSecondsLeft;
            UpdateView();
        }

        private void OnHireStaffButtonClicked()
        {
            PlayButtonSound();
            
            if (_targetCashDeskModel != null)
            {
                _eventBus.Dispatch(new CashDeskHireStaffButtonClickedEvent(_targetCashDeskModel));
            }
        }

        private void OnPlayerFocusChanged(bool isFocused)
        {
            UpdatePanelViewState();
        }

        private void OnNearShopObjectsUpdated()
        {
            UpdatePanelViewState();
        }

        private void OnLevelChanged(int level)
        {
            UpdatePanelViewState();
        }

        private void UpdatePanelViewState()
        {
            if (_playerCharModel.NearCashDesk != null
                && _playerModelHolder.PlayerModel.Level >= Constants.MinLevelForCashDeskUpgrades
                && _playerFocusProvider.IsPlayerFocused)
            {
                PanelView.SetStaffTitleText(
                    _localizationProvider.GetLocale(Constants.LocalizationBottomPanelCashDeskStaffTitle));

                ProcessNewTargetCashDesk(_playerCharModel.NearCashDesk);

                SlideUp();
            }
            else
            {
                ResetTargetCashDesk();

                SlideDown();
            }
        }

        private void ResetTargetCashDesk()
        {
            if (_targetCashDeskModel == null) return;

            UnsubscribeFromCashDesk(_targetCashDeskModel);

            _targetCashDeskModel = null;
            _workSecondsLeftTemp = 0;
        }

        private void ProcessNewTargetCashDesk(CashDeskModel cashDeskModel)
        {
            ResetTargetCashDesk();
            
            _targetCashDeskModel = cashDeskModel;

            UpdateView();

            SubscribeOnCashDesk(_targetCashDeskModel);
        }
        
        private void SubscribeOnCashDesk(CashDeskModel targetCashDesk)
        {
            targetCashDesk.StaffAdded += OnStaffAdded;
            targetCashDesk.StaffRemoved += OnStaffRemoved;
        }
        
        private void UnsubscribeFromCashDesk(CashDeskModel targetCashDesk)
        {
            targetCashDesk.StaffAdded -= OnStaffAdded;
            targetCashDesk.StaffRemoved -= OnStaffRemoved;
        }

        private void OnStaffAdded(CashDeskStaffModel cashDeskStaffModel)
        {
            UpdateView();

            RequestFlyingText(Constants.LocalizationHired, PanelView.HireStaffButtonView.transform);
        }

        private void OnStaffRemoved(CashDeskStaffModel cashDeskStaffModel)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            if (_targetCashDeskModel != null)
            {
                DisplayStaff(_targetCashDeskModel);
                
                var hireCost = _hireStaffCostProvider.GetCashDeskHireStaffCost(_targetCashDeskModel);
                ButtonsHelper.SetupHireButtonText(PanelView.HireStaffButtonView, _targetCashDeskModel.CashDeskStaffModel, hireCost);
            }
        }

        private void DisplayStaff(CashDeskModel cashDeskModel)
        {
            var staffModel = cashDeskModel.CashDeskStaffModel;
            
            var isStaffExists = staffModel != null;
            PanelView.SetStaffEnabled(isStaffExists);

            if (staffModel != null)
            {
                var workTimeText = FormattingHelper.ToTimeFormatMinSec(staffModel.WorkSecondsLeft);
                PanelView.SetStaffWorkTimerText(workTimeText);

                var clockColor = StaffCharHelper.GetClockColorByPercent(
                    (float)_targetCashDeskModel.CashDeskStaffModel.WorkSecondsLeft /
                    _playerModel.StaffWorkTimeSeconds);

                PanelView.SetClockColor(clockColor);
                PanelView.SetStaffWorkTimerTextColor(clockColor);
            }
        }
    }
}