using UnityEngine;

namespace Data
{
    public static class Constants
    {
        public static readonly Vector2Int ShopObjectRelativeToBuildPointOffset = Vector2Int.left;
        public static readonly Vector2Int CashDeskStaffPositionOffset = Vector2Int.left + Vector2Int.down;
        
        public const int TruckArrivingDuration = 2;
        public const int ProductsAmountInBox = 4;
        public const int MinDeliverTimeSeconds = 5;
        public const int YTopWalkableCoordForCustomers = -10;
        public const string GeneralTopSortingLayerName = "GeneralTop";
        public const int MinLevelForCashDeskUpgrades = 2;
        public const int MinLevelForTruckPointUpgrades = 2;
        public const int MinLevelForShelfUpgrades = 1;
        public const int MinLevelForShopExpand = 2;
        public const int MinLevelForLeaderboardButton = 5;
        public const int ExpandCellsAmount = 3;
        public const int ExpandPointFreeCoord = 3;
        public const int HireByAdvertWorkTimeMultiplier = 5;
        public const int CellDistanceToSound = 10;
        public const int SecondsInMinute = 60;
        public const int ManUISpriteKeyOffset = 1000;
        public const string AssetBundleUpliftMusicName = "music_uplift";
        public const string PlayerDataKey = "data";
        public const string PlayerScoreKey = "score";

        public const string LocalizationKeyWaitLoading = "wait_loading";
        public const string LocalizationKeyMarketLevelPrefix = "market_level_";
        public const string LocalizationBottomPanelDeliveryTitle = "bottom_panel_delivery_title";
        public const string LocalizationBottomPanelStaffTitle = "bottom_panel_staff_title";
        public const string LocalizationBottomPanelCashDeskStaffTitle = "bottom_panel_cash_desk_staff_title";
        public const string LocalizationHireButton = "hire_button";
        public const string LocalizationProlongButton = "prolong_button";
        public const string LocalizationUpgradeButton = "upgrade_button";
        public const string LocalizationMaxUpgrade = "max_upgrade";
        public const string LocalizationSecondsShortPostfix = "seconds_short_postfix";
        public const string LocalizationUpgraded = "upgraded";
        public const string LocalizationHired = "hired";
        public const string LocalizationProlonged = "prolonged";
        public const string LocalizationUpgradeShelf = "upgrade_shelf";
        public const string LocalizationKeyLevel = "level";
        public const string LocalizationKeyNewLevelReached = "new_level_reached";
        public const string LocalizationTutorialHowToMoveMessageKey = "tutorial_how_to_move_text";
        public const string LocalizationTutorialBuildCashDeskMessageKey = "tutorial_build_cash_desk_text";
        public const string LocalizationTutorialBuildShelfMessageKey = "tutorial_build_shelf_text";
        public const string LocalizationTutorialBuildTruckPointMessageKey = "tutorial_build_truck_point_text";
        public const string LocalizationTutorialTakeProductsFromTruckPointMessageKey = "tutorial_take_products_from_truck_point_text";
        public const string LocalizationTutorialTakeProductsFromTruckPointSecondTimeMessageKey = "tutorial_take_products_from_truck_point_2_text";
        public const string LocalizationTutorialPutProductsOnShelfMessageKey = "tutorial_put_products_on_shelf_text";
        public const string LocalizationTutorialPutProductsOnShelfSecondTimeMessageKey = "tutorial_put_products_on_shelf_2_text";
        public const string LocalizationTutorialMoveToCashDeskMessageKey = "tutorial_move_to_cash_desk_text";
        public const string LocalizationTutorialHireCashDeskStaffMessageKey = "tutorial_hire_cash_desk_staff_text";
        public const string LocalizationTutorialUpgradeTruckPointMessageKey = "tutorial_upgrade_truck_point_text";
        public const string LocalizationTutorialInteriorButtonMessageKey = "tutorial_interior_button_text";
        public const string LocalizationTutorialDressesButtonMessageKey = "tutorial_dresses_button_text";
        public const string LocalizationTutorialLeaderboardButtonMessageKey = "tutorial_leaderboard_button_text";
        public const string LocalizationChooseMessageKey = "choose";
        public const string LocalizationChosenMessageKey = "choosen";
        public const string LocalizationBuyMessageKey = "buy";
        public const string LocalizationInteriorPopupTitleKey = "interior_popup_title";
        public const string LocalizationInteriorPopupWallsTabTitleKey = "interior_popup_wall_tab_title";
        public const string LocalizationInteriorPopupFloorsTabTitleKey = "interior_popup_floors_tab_title";
        public const string LocalizationDressesPopupTitleKey = "dresses_popup_title";
        public const string LocalizationDressesPopupTopDressTabTitleKey = "dresses_popup_top_dress_tab_title";
        public const string LocalizationDressesPopupBottomDressTabTitleKey = "dresses_popup_bottom_dress_tab_title";
        public const string LocalizationDressesPopupHairTabTitleKey = "dresses_popup_hair_tab_title";
        public const string LocalizationDressesPopupGlassesTabTitleKey = "dresses_popup_glasses_tab_title";
        public const string LocalizationKeyBought = "bought";
        public const string LocalizationKeyMinutesShort = "minutes_short";
        public const string LocalizationKeyAdsOfferHireAllStaff = "ads_offer_hire_all_staff";
        public const string LocalizationKeySettingsPopupTitle = "settings_popup_title";
        public const string LocalizationKeySettingsPopupSounds = "settings_popup_sounds";
        public const string LocalizationKeySettingsPopupMusic = "settings_popup_music";
        public const string LocalizationKeySettingsPopupResetButton = "settings_popup_reset_button";
        public const string LocalizationKeySettingsPopupId = "settings_popup_id";
        public const string LocalizationKeyYesNoPopupDefaultTitle = "yes_no_popup_title_default";
        public const string LocalizationKeyYesNoPopupResetDataMessage = "yes_no_popup_message_reset";
        public const string LocalizationKeyYes = "yes";
        public const string LocalizationKeyNo = "no";
        public const string LocalizationKeySuccess = "success";
        public const string LocalizationKeyResetDataSuccessMessage = "reset_data_success_message";
        public const string LocalizationKeyResetDataReloadButtonText = "reset_data_reload_button";

        private const string ColdPrefabsFolderPath = "Prefabs/Cold Prefabs";
        public static readonly string TutorialHowToMovePath = $"{ColdPrefabsFolderPath}/TutorialHowToMove";
        public static readonly string TutorialDefaultStepWithTextPath = $"{ColdPrefabsFolderPath}/TutorialDefaultStep";
        public static readonly string TutorialStepPointUIPath = $"{ColdPrefabsFolderPath}/TutorialPointUIStep";

        public const string TextIconMoney = "<sprite name=\"money\">";
        public const string TextIconAds = "<sprite name=\"ads\">";
        public const string TextIconStar = "<sprite name=\"star\">";

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