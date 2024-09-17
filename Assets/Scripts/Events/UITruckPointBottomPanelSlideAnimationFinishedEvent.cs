namespace Events
{
    public struct UITruckPointBottomPanelSlideAnimationFinishedEvent
    {
        public readonly bool IsSlideUp;
        
        public UITruckPointBottomPanelSlideAnimationFinishedEvent(bool isSlideUp)
        {
            IsSlideUp = isSlideUp;
        }
    }
}