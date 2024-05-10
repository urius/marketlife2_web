using UnityEngine;

namespace Events
{
    public struct RequestPlayerCellChangeEvent
    {
        public readonly Vector2Int CellCoords;

        public RequestPlayerCellChangeEvent(Vector2Int cellCoords)
        {
            CellCoords = cellCoords;
        }
    }
}