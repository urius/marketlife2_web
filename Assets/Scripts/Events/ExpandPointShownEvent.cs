using UnityEngine;

namespace Events
{
    public struct ExpandPointShownEvent
    {
        public readonly Vector2Int CellPosition;

        public ExpandPointShownEvent(Vector2Int cellPosition)
        {
            CellPosition = cellPosition;
        }
    }
}