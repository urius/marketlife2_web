namespace Utils
{
    public static class InteriorCostHelper
    {
        public static int GetWallCostForLevel(int level)
        {
            return 5 + level * 2;
        }
        
        public static int GetFloorCostForLevel(int level)
        {
            return 7 + level * 3;
        }
    }
}