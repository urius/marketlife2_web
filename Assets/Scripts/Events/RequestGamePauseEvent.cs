namespace Events
{
    public struct RequestGamePauseEvent
    {
        public readonly string RequesterId;
        public readonly bool IsPaused;
        public readonly bool NeedMute;

        public RequestGamePauseEvent(string requesterId, bool isPaused, bool needMute = false)
        {
            RequesterId = requesterId;
            IsPaused = isPaused;
            NeedMute = needMute;
        }
    }
}