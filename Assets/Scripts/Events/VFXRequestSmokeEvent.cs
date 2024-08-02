using UnityEngine;

namespace Events
{
    public struct VFXRequestSmokeEvent
    {
        public readonly Vector2Int CellPosition;
        public readonly bool UseBigSmoke;

        public VFXRequestSmokeEvent(Vector2Int cellPosition, bool useBigSmoke = false)
        {
            CellPosition = cellPosition;
            UseBigSmoke = useBigSmoke;
        }
    }
}