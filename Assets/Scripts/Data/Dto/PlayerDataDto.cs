using System;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerDataDto
    {
        public AudioSettingsDto AudioSettings;
        public PlayerUIFlagsDto UIFlags;
        
        public int Money;
        public int Level;
        public int StaffWorkTimeSeconds;
        public ShopDataDto ShopData;
        public PlayerCharDataDto PlayerCharData;
        public WallType[] UnlockedWalls;
        public FloorType[] UnlockedFloors;

        public TutorialStep[] TutorialSteps;
        
        public PlayerStatsDto Stats;
    }
}