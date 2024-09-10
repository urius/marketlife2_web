using System.Collections.Generic;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;

namespace Systems.Tutorial
{
    public class TutorialSystem : ISystem
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
    
        private readonly TutorialStep[][] _tutorialSequences = TutorialSteps.TutorialSequences;

        private PlayerModel _playerModel;

        public void Start()
        {
            _playerModel = _playerModelHolder.PlayerModel;
        
            UpdateOpenedSteps();
        
            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<TutorialStepFinishedEvent>(OnTutorialStepFinishedEvent);
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<TutorialStepFinishedEvent>(OnTutorialStepFinishedEvent);
        }

        private void OnTutorialStepFinishedEvent(TutorialStepFinishedEvent e)
        {
            var tutorialStep = e.TutorialStep;
            
            _playerModel.RemoveOpenTutorialStep(tutorialStep);
            _playerModel.AddPassedTutorialStep(tutorialStep);

            UpdateOpenedSteps();
        }

        private void UpdateOpenedSteps()
        {
            foreach (var sequence in _tutorialSequences)
            {
                if (TryGetOpenTutorialStepInSequence(sequence, out var step))
                {
                    _playerModel.AddOpenTutorialStep(step);
                }
            }
        }

        private bool TryGetOpenTutorialStepInSequence(IReadOnlyList<TutorialStep> sequence, out TutorialStep result)
        {
            result = default;

            foreach (var step in sequence)
            {
                if (_playerModel.IsTutorialStepPassed(step)) continue;
                
                result = step;
                
                return true;
            }

            return false;
        }
    }
}