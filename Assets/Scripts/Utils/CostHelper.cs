namespace Utils
{
    public static class CostHelper
    {
        public static int GetWallCostForLevel(int level)
        {
            return 5 + level * 2;
        }
        
        public static int GetFloorCostForLevel(int level)
        {
            return 7 + level * 3;
        }

        public static int GetTopDressCostForLevel(int level)
        {
            return 5 + level * 3;
        }

        public static int GetBottomDressCostForLevel(int level)
        {
            return 3 + level * 3;
        }

        public static int GetHairCostForLevel(int level)
        {
            return 10 + level * 2;
        }

        public static int GetGlassesCostForLevel(int level)
        {
            return 5 + level * 5;
        }
    }
}