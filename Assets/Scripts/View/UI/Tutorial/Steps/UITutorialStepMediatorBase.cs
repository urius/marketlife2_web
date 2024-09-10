using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public abstract class UITutorialStepMediatorBase : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();

        private bool _stepFinishedIsDispatched;
        
        public TutorialStep TutorialStep { get; private set; }
        
        public void Mediate(Transform transform, TutorialStep tutorialStep)
        {
            TutorialStep = tutorialStep;
            
            base.Mediate(transform);
        }

        protected void DispatchStepFinished()
        {
            if (_stepFinishedIsDispatched) return;
            
            _stepFinishedIsDispatched = true;
            _eventBus.Dispatch(new TutorialStepFinishedEvent(TutorialStep));
        }
    }
}