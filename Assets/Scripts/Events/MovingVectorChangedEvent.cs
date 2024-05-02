using UnityEngine;

namespace Events
{
    public struct MovingVectorChangedEvent
    {
        public readonly Vector2 Direction;

        public MovingVectorChangedEvent(Vector2 direction)
        {
            Direction = direction;
        }
    }
}