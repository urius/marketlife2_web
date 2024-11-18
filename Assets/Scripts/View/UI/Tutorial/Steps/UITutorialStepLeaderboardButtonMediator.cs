using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepLeaderboardButtonMediator : UITutorialStepMediatorBase
    {
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly ISharedFlagsHolder _sharedFlagsHolder = Instance.Get<ISharedFlagsHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private UITutorialStepUIPointerView _view;

        protected override void MediateInternal()
        {
        }

        protected override bool CheckStepConditions()
        {
            var leaderboardButtonShownFlag = _sharedFlagsHolder.Get(SharedFlagKey.UITopPanelLeaderboardButtonShown);
            
            return leaderboardButtonShownFlag;
        }

        protected override void ActivateStep()
        {
            _view = InstantiateColdPrefab<UITutorialStepUIPointerView>(Constants.TutorialStepPointUIPath);
            
            _view.ToTopRightSideState();

            var leaderboardButtonTransform = _sharedViewsDataHolder.GetTopPanelLeaderboardButtonTransform();
            
            _view.SetText(_localizationProvider.GetLocale(Constants.LocalizationTutorialLeaderboardButtonMessageKey));
            
            _view.SetPointerToPosition(leaderboardButtonTransform.position, new Vector2(0, -10));

            SubscribeOnActivate();
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

        private void SubscribeOnActivate()
        {
            _eventBus.Subscribe<UILeaderboardButtonClickedEvent>(OnUILeaderboardButtonClickedEvent);
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<UILeaderboardButtonClickedEvent>(OnUILeaderboardButtonClickedEvent);
        }

        private void OnUILeaderboardButtonClickedEvent(UILeaderboardButtonClickedEvent e)
        {
            DispatchStepFinished();
        }
    }
}