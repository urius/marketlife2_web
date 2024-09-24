using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using View.UI.BottomPanel;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepUpgradeTruckPointMediator : UITutorialStepMediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private bool _panelSlideUpFinishedFlag = false;
        private int _showsCounter = 0;
        private IUITruckPointPanelTransformsProvider _panelTransformsProvider;
        private UITutorialStepUIPointerView _view;

        protected override void MediateInternal()
        {
            _panelTransformsProvider = _sharedViewsDataHolder.GetTruckPointPanelTransformsProvider();
            
            Subscribe();
        }

        protected override bool CheckStepConditions()
        {
            return _panelSlideUpFinishedFlag;
        }

        protected override void ActivateStep()
        {
            _view = InstantiateColdPrefab<UITutorialStepUIPointerView>(Constants.TutorialStepPointUIPath);
            
            _view.ToBottomRightSideState();

            var upgradeButtonTransform = _panelTransformsProvider.UpgradeButtonTransform;
            
            _view.SetText(_localizationProvider.GetLocale(Constants.LocalizationTutorialUpgradeTruckPointMessageKey));
            
            _view.SetPointerToPosition(upgradeButtonTransform.position);

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
            _eventBus.Subscribe<UITruckPointBottomPanelSlideAnimationFinishedEvent>(OnUITruckPointBottomPanelSlideAnimationFinished);
        }

        private void SubscribeOnActivation()
        {
            _eventBus.Subscribe<UpgradeTruckPointButtonClickedEvent>(OnUpgradeTruckPointButtonClickedEvent);
        }

        private void Unsubscribe()
        {            
            _eventBus.Unsubscribe<UITruckPointBottomPanelSlideAnimationFinishedEvent>(OnUITruckPointBottomPanelSlideAnimationFinished);
            _eventBus.Unsubscribe<UpgradeTruckPointButtonClickedEvent>(OnUpgradeTruckPointButtonClickedEvent);
        }

        private void OnUpgradeTruckPointButtonClickedEvent(UpgradeTruckPointButtonClickedEvent e)
        {
            DispatchStepFinished();
        }

        private void OnUITruckPointBottomPanelSlideAnimationFinished(UITruckPointBottomPanelSlideAnimationFinishedEvent e)
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
                else if (_showsCounter > 1)
                {
                    DispatchStepFinished();
                }
            }
        }
    }
}