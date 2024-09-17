namespace Events
{
    public struct UICashDeskBottomPanelSlideAnimationFinishedEvent
    {
        public readonly bool IsSlideUp;

        public UICashDeskBottomPanelSlideAnimationFinishedEvent(bool isSlideUp)
        {
            IsSlideUp = isSlideUp;
        }
    }
}