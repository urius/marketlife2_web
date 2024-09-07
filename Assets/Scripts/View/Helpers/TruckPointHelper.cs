using UnityEngine;

namespace View.Helpers
{
    public static class TruckPointHelper
    {
        public static readonly Vector2Int PrimaryInteractionCellOffset = Vector2Int.right;
        public static readonly Vector2Int SecondaryInteractionCellOffset = PrimaryInteractionCellOffset + Vector2Int.down;

        public static Vector2Int GetClosestInteractionCell(Vector2Int truckPointPosition, Vector2Int cellToCheck)
        {
            var interactionCell1 = truckPointPosition + PrimaryInteractionCellOffset;
            var interactionCell2 = truckPointPosition + SecondaryInteractionCellOffset;
            
            var distance1 = Vector2Int.Distance(interactionCell1, cellToCheck);
            var distance2 = Vector2Int.Distance(interactionCell2, cellToCheck);

            return distance1 < distance2 ? interactionCell1 : interactionCell2;
        }

        public static bool IsOnInteractionPoint(Vector2Int truckPointPosition, Vector2Int targetCellCoords)
        {
            var interactionCell1 = truckPointPosition + PrimaryInteractionCellOffset;
            var interactionCell2 = truckPointPosition + SecondaryInteractionCellOffset;

            return targetCellCoords == interactionCell1 || targetCellCoords == interactionCell2;
        }
        
        public static Vector2Int GetTruckPointWaitingPoint(Vector2Int truckPointPosition)
        {
            return truckPointPosition + SecondaryInteractionCellOffset + Vector2Int.down;
        }
    }
}