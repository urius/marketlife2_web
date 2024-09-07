using System;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerDataDto
    {
        public int Money;
        public int Level;
        public int StaffWorkTimeSeconds;
        public ShopDataDto ShopData;
        public PlayerCharDataDto PlayerCharData;
    }
}