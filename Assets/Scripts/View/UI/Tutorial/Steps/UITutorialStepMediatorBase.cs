using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public abstract class UITutorialStepMediatorBase : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();

        private bool _stepFinishedIsDispatched;
        
        public TutorialStep TutorialStep { get; private set; }
        
        public void Mediate(Transform transform, TutorialStep tutorialStep)
        {
            TutorialStep = tutorialStep;
            
            base.Mediate(transform);

            if (TryActivateStep() == false)
            {
                _updatesProvider.QuarterSecondPassed += OnQuarterSecondPassed;
            }
        }

        protected override void MediateInternal()
        {
            //empty
        }

        public override void Unmediate()
        {
            _updatesProvider.QuarterSecondPassed -= OnQuarterSecondPassed;
            
            base.Unmediate();
        }

        private void OnQuarterSecondPassed()
        {
            if (TryActivateStep())
            {
                _updatesProvider.QuarterSecondPassed -= OnQuarterSecondPassed;
            }
        }

        protected abstract bool CheckStepConditions();
        protected abstract void ActivateStep();
        

        protected void DispatchStepFinished()
        {
            if (_stepFinishedIsDispatched) return;
            
            _stepFinishedIsDispatched = true;
            _eventBus.Dispatch(new TutorialStepFinishedEvent(TutorialStep));
        }

        private bool TryActivateStep()
        {
            var checkConditionsResult = CheckStepConditions();
            
            if (CheckStepConditions())
            {
                ActivateStep();
            }

            return checkConditionsResult;
        }
    }
}