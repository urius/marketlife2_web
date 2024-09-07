using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.People;
using Model.ShopObjects;
using View.Helpers;

namespace View.UI.BottomPanel
{
    public class UICashDeskPanelMediator : UIBottomPanelMediatorBase<UICashDeskPanelView>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IHireStaffCostProvider _hireStaffCostProvider = Instance.Get<IHireStaffCostProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private PlayerCharModel _playerCharModel;
        private string _secondsPostfix;
        private CashDeskModel _targetCashDeskModel;
        private int _workSecondsLeftTemp;

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
            PanelView.HireStaffButtonClicked += OnHireStaffButtonClicked;
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
        }

        private void Unsubscribe()
        {
            _playerCharModel.NearShopObjectsUpdated -= OnNearShopObjectsUpdated;
            PanelView.HireStaffButtonClicked -= OnHireStaffButtonClicked;
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
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
            if (_targetCashDeskModel != null)
            {
                _eventBus.Dispatch(new CashDeskHireStaffButtonClickedEvent(_targetCashDeskModel));
            }
        }

        private void OnNearShopObjectsUpdated()
        {
            if (_playerCharModel.NearCashDesk != null 
                && _playerModelHolder.PlayerModel.Level >= Constants.MinLevelForCashDeskUpgrades)
            {
                _secondsPostfix = _localizationProvider.GetLocale(Constants.LocalizationSecondsShortPostfix);
                
                PanelView.SetStaffTitleText(_localizationProvider.GetLocale(Constants.LocalizationBottomPanelCashDeskStaffTitle));
                
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
                PanelView.SetStaffWorkTimerText($"{staffModel.WorkSecondsLeft}{_secondsPostfix}");
            }
        }
    }
}