using UnityEngine;

namespace Events
{
    public struct RequestRemoveCompassEvent
    {
        public Vector2Int TargetCellCoord;

        public RequestRemoveCompassEvent(Vector2Int targetCellCoord)
        {
            TargetCellCoord = targetCellCoord;
        }
    }
}