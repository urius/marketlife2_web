using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using View.UI.BottomPanel;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepHireCashDeskStaffMediator : UITutorialStepMediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private bool _panelSlideUpFinishedFlag = false;
        private int _showsCounter = 0;
        private UITutorialStepUIPointerView _view;
        private IUICashDeskPanelTransformsProvider _panelTransformsProvider;

        protected override void MediateInternal()
        {
            _panelTransformsProvider = _sharedViewsDataHolder.GetCashDeskPanelTransformsProvider();
            
            Subscribe();
        }

        protected override bool CheckStepConditions()
        {
            return _panelSlideUpFinishedFlag;
        }

        protected override void ActivateStep()
        {
            _view = InstantiateColdPrefab<UITutorialStepUIPointerView>(Constants.TutorialStepPointUIPath);
            
            _view.ToBottomLeftSideState();

            var hireButtonTransform = _panelTransformsProvider.HireButtonTransform;
            
            _view.SetText(_localizationProvider.GetLocale(Constants.LocalizationTutorialHireCashDeskStaffMessageKey));
            
            _view.SetPointerToPosition(hireButtonTransform.position);

            _showsCounter++;

            SubscribeOnActivation();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();

            if (_view != null)
            {
                Destroy(_view);
                _view = null;
            }
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<UICashDeskBottomPanelSlideAnimationFinishedEvent>(OnUICashDeskBottomPanelSlideUpFinished);
        }

        private void SubscribeOnActivation()
        {
            _eventBus.Subscribe<CashDeskHireStaffButtonClickedEvent>(OnCashDeskHireStaffButtonClicked);
        }

        private void OnCashDeskHireStaffButtonClicked(CashDeskHireStaffButtonClickedEvent e)
        {
            DispatchStepFinished();
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<UICashDeskBottomPanelSlideAnimationFinishedEvent>(OnUICashDeskBottomPanelSlideUpFinished);
            _eventBus.Unsubscribe<CashDeskHireStaffButtonClickedEvent>(OnCashDeskHireStaffButtonClicked);
        }

        private void OnUICashDeskBottomPanelSlideUpFinished(UICashDeskBottomPanelSlideAnimationFinishedEvent e)
        {
            var isSlideUp = e.IsSlideUp;
            _panelSlideUpFinishedFlag = isSlideUp;

            if (_view != null)
            {
                _view.gameObject.SetActive(_panelSlideUpFinishedFlag);

                if (isSlideUp)
                {
                    _showsCounter++;
                }
                else if (_showsCounter > 5)
                {
                    DispatchStepFinished();
                }
            }
        }
    }
}