namespace Events
{
    public struct RequestGamePauseEvent
    {
        public readonly string RequesterId;
        public readonly bool IsPaused;

        public RequestGamePauseEvent(string requesterId, bool isPaused)
        {
            RequesterId = requesterId;
            IsPaused = isPaused;
        }
    }
}