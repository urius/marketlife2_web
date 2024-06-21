using UnityEngine;

namespace Data
{
    public class Constants
    {
        public const int TruckArrivingDuration = 2;
        public const int ProductsAmountInBox = 4;
        public const string GeneralTopSortingLayerName = "GeneralTop";

        public static readonly Vector2Int[] NearCells = new[]
        {
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
            new Vector2Int(1, -1),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
        };
    }
}