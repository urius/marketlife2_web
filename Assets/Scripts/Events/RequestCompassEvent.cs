using UnityEngine;

namespace Events
{
    public struct RequestCompassEvent
    {
        public Vector2Int TargetCellCoord;

        public RequestCompassEvent(Vector2Int targetCellCoord)
        {
            TargetCellCoord = targetCellCoord;
        }
    }
}