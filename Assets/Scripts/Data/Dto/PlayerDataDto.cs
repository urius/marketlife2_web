using System;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerDataDto
    {
        public ShopDataDto ShopData;
        public int Money;
        public int Level;
        public int StaffWorkTimeSeconds;
        public WallType[] BoughtWalls;
        public FloorType[] BoughtFloors;
        public PlayerCharDataDto PlayerCharData;

        public int[] PassedTutorialSteps;
        
        public AudioSettingsDto AudioSettings;
        public PlayerUIFlagsDto UIFlags;
        
        public PlayerStatsDto Stats;

        public PlayerDataDto(
            ShopDataDto shopData,
            int money,
            int level,
            int staffWorkTimeSeconds,
            WallType[] boughtWalls,
            FloorType[] boughtFloors,
            PlayerCharDataDto playerCharData,
            int[] passedTutorialSteps,
            AudioSettingsDto audioSettings,
            PlayerUIFlagsDto uiFlags,
            PlayerStatsDto stats)
        {
            ShopData = shopData;
            Money = money;
            Level = level;
            StaffWorkTimeSeconds = staffWorkTimeSeconds;
            BoughtWalls = boughtWalls;
            BoughtFloors = boughtFloors;
            PlayerCharData = playerCharData;
            PassedTutorialSteps = passedTutorialSteps;
            AudioSettings = audioSettings;
            UIFlags = uiFlags;
            Stats = stats;
        }
    }
}