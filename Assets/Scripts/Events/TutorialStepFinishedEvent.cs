using Data;

namespace Events
{
    public struct TutorialStepFinishedEvent
    {
        public readonly TutorialStep TutorialStep;

        public TutorialStepFinishedEvent(TutorialStep tutorialStep)
        {
            TutorialStep = tutorialStep;
        }
    }
}