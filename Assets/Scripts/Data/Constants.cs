using UnityEngine;

namespace Data
{
    public static class Constants
    {
        public static readonly Vector2Int ShopObjectRelativeToBuildPointOffset = Vector2Int.left;
        
        public const int TruckArrivingDuration = 2;
        public const int ProductsAmountInBox = 4;
        public const int YTopWalkableCoordForCustomers = -10;
        public const string GeneralTopSortingLayerName = "GeneralTop";

        public const string LocalizationKeyMarketLevelPrefix = "market_level_";

        public static readonly Vector2Int[] NearCells8 = new[]
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
        
        public static readonly Vector2Int[] NearCells4 = new[]
        {
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
        };
    }
}