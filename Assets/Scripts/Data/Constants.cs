using UnityEngine;

namespace Data
{
    public static class Constants
    {
        public static readonly Vector2Int ShopObjectRelativeToBuildPointOffset = Vector2Int.left;
        
        public const int TruckArrivingDuration = 2;
        public const int ProductsAmountInBox = 4;
        public const int MinDeliverTimeSeconds = 5;
        public const int YTopWalkableCoordForCustomers = -10;
        public const string GeneralTopSortingLayerName = "GeneralTop";

        public const string LocalizationKeyMarketLevelPrefix = "market_level_";
        public const string LocalizationBottomPanelDeliveryTitle = "bottom_panel_delivery_title";
        public const string LocalizationBottomPanelStaffTitle = "bottom_panel_staff_title";
        public const string LocalizationHireButton = "hire_button";
        public const string LocalizationUpgradeButton = "upgrade_button";
        public const string LocalizationMaxUpgrade = "max_upgrade";
        public const string LocalizationSecondsShortPostfix = "seconds_short_postfix";
        public const string LocalizationUpgraded = "upgraded";
        public const string LocalizationHired = "hired";
        public const string LocalizationUpgradeShelf = "upgrade_shelf";
        
        public const string TextIconMoney = "<sprite name=\"money\">";
        public const string TextIconAds = "<sprite name=\"ads\">";

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