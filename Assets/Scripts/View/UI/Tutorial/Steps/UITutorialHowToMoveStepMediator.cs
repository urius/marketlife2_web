using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialHowToMoveStepMediator : UITutorialStepMediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private static readonly string ColdPrefabPath = Constants.TutorialHowToMovePath;
        
        private UITutorialHowToMoveStepView _stepView;
        private int _playerPositionChangesCounter;

        protected override void MediateInternal()
        {
        }

        protected override void ActivateStep()
        {
            _stepView = InstantiateColdPrefab<UITutorialHowToMoveStepView>(ColdPrefabPath);
            
            _stepView.SetMessageText(_localizationProvider.GetLocale(Constants.LocalizationTutorialHowToMoveMessageKey));

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();

            if (_stepView != null)
            {
                Destroy(_stepView);
                _stepView = null;
            }
        }

        protected override bool CheckStepConditions()
        {
            return true;
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<PlayerCharPositionChangedEvent>(OnPlayerCharPositionChanged);
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<PlayerCharPositionChangedEvent>(OnPlayerCharPositionChanged);
        }

        private void OnPlayerCharPositionChanged(PlayerCharPositionChangedEvent e)
        {
            _playerPositionChangesCounter++;
            
            if (_playerPositionChangesCounter > 30)
            {
                _eventBus.Unsubscribe<PlayerCharPositionChangedEvent>(OnPlayerCharPositionChanged);

                DispatchStepFinished();
            }
        }
    }
}