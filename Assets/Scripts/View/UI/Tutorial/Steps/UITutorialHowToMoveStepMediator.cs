using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialHowToMoveStepMediator : UITutorialStepMediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private static readonly string ColdPrefabPath = Constants.TutorialHowToMoveColdPrefabPath;
        
        private UITutorialHowToMoveStepView _stepView;
        private int _playerPositionChangesCounter;

        protected override void MediateInternal()
        {
            _stepView = InstantiateColdPrefab<UITutorialHowToMoveStepView>(ColdPrefabPath);

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_stepView);
            _stepView = null;
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