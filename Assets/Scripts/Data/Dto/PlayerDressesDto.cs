using System;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerDressesDto
    {
        public ManSpriteType TopDressType;
        public ManSpriteType BottomDressType;
        public ManSpriteType HairType;
        public ManSpriteType GlassesType;

        public ManSpriteType[] BoughtTopDresses;
        public ManSpriteType[] BoughtBottomDresses;
        public ManSpriteType[] BoughtHairs;
        public ManSpriteType[] BoughtGlasses;

        public PlayerDressesDto(
            ManSpriteType topDressType,
            ManSpriteType bottomDressType,
            ManSpriteType hairType,
            ManSpriteType glassesType,
            ManSpriteType[] boughtTopDresses,
            ManSpriteType[] boughtBottomDresses,
            ManSpriteType[] boughtHairs,
            ManSpriteType[] boughtGlasses)
        {
            TopDressType = topDressType;
            BottomDressType = bottomDressType;
            HairType = hairType;
            GlassesType = glassesType;
            BoughtTopDresses = boughtTopDresses;
            BoughtBottomDresses = boughtBottomDresses;
            BoughtHairs = boughtHairs;
            BoughtGlasses = boughtGlasses;
        }
    }
}