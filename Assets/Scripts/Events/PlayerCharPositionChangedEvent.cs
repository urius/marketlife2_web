using UnityEngine;

namespace Events
{
    public struct PlayerCharPositionChangedEvent
    {
        public readonly Vector2 Position;

        public PlayerCharPositionChangedEvent(Vector2 position)
        {
            Position = position;
        }
    }
}