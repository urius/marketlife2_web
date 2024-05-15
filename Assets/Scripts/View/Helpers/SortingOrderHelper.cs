using UnityEngine;

namespace View.Helpers
{
    public static class SortingOrderHelper
    {
        public static readonly Vector2Int[] NearOffsets4 = new[]
            { new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0) };
        
        public static int GetDefaultSortingOrderByCoords(Vector2Int cellCoords)
        {
            return (cellCoords.x + cellCoords.y) * 5;
        }

        public static int GetSortingOrderNearShopObject(Vector2Int[] shopObjectOwnedCells, Vector2Int targetCoords)
        {
            var offset = 0;
            var firstCell = shopObjectOwnedCells[0];
            
            var minY = firstCell.y;
            var maxY = firstCell.y;
            var minX = firstCell.x;
            var maxX = firstCell.x;

            foreach (var ownedCell in shopObjectOwnedCells)
            {
                if (ownedCell.y < minY) minY = ownedCell.y;
                if (ownedCell.y > maxY) maxY = ownedCell.y;
                if (ownedCell.x < minX) minX = ownedCell.x;
                if (ownedCell.x > maxX) maxX = ownedCell.x;
            }

            if (targetCoords.y < minY || targetCoords.x < minX) offset--;
            if (targetCoords.y > maxY || targetCoords.x > maxX) offset++;

            return GetDefaultSortingOrderByCoords(targetCoords) + offset;
        }
    }
}