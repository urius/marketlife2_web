using System;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerDataDto
    {
        public int Money;
        public ShopDataDto ShopData;
        public PlayerCharDataDto PlayerCharData;
    }
}