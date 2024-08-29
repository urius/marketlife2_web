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
        private int _currentHireStaffCost;

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
            _updatesProvider.SecondPassed += OnSecondPassed;
        }

        private void Unsubscribe()
        {
            _playerCharModel.NearShopObjectsUpdated -= OnNearShopObjectsUpdated;
            PanelView.HireStaffButtonClicked -= OnHireStaffButtonClicked;
            _updatesProvider.SecondPassed -= OnSecondPassed;
        }

        private void OnSecondPassed()
        {
            UpdateView();
        }

        private void OnHireStaffButtonClicked()
        {
            if (_targetCashDeskModel != null)
            {
                _eventBus.Dispatch(new CashDeskHireStaffButtonClickedEvent(_targetCashDeskModel, _currentHireStaffCost));
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
            
            _eventBus.Dispatch(
                new UIRequestFlyingTextEvent(
                    _localizationProvider.GetLocale(Constants.LocalizationHired),
                    PanelView.HireStaffButtonView.transform.position));
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
                SetupHireButton(_targetCashDeskModel);
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
                PanelView.SetStaffWorkTimeProgress((float)staffModel.WorkSecondsLeft / staffModel.WorkSecondsSetting);
            }
        }

        private void SetupHireButton(CashDeskModel cashDeskModel)
        {
            var hireStaffCost = _hireStaffCostProvider.GetCashDeskHireStaffCost();
            _currentHireStaffCost = hireStaffCost;

            PanelView.SetHireStaffButtonInteractable(cashDeskModel.CashDeskStaffModel == null);
            
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