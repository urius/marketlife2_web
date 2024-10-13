using System;

namespace Model
{
    public class PlayerUIFlagsModel
    {
        public event Action<bool> WallsFlagChanged;
        public event Action<bool> FloorsFlagChanged;
        public event Action DressesFlagChanged;

        public PlayerUIFlagsModel(bool haveNewWalls, bool haveNewFloors)
        {
            HaveNewWalls = haveNewWalls;
            HaveNewFloors = haveNewFloors;
        }
        
        public bool HaveNewWalls { get; private set; }
        public bool HaveNewFloors { get; private set; }
        public bool HaveNewTopDresses { get; private set; }
        public bool HaveNewBottomDresses { get; private set; }
        public bool HaveNewHairs { get; private set; }
        public bool HaveNewGlasses { get; private set; }
        
        public void SetNewTopDressesFlag(bool newFlagValue)
        {
            if (HaveNewTopDresses != newFlagValue)
            {
                HaveNewTopDresses = newFlagValue;
                
                DressesFlagChanged?.Invoke();
            }
        }
        
        public void SetNewBottomDressesFlag(bool newFlagValue)
        {
            if (HaveNewBottomDresses != newFlagValue)
            {
                HaveNewBottomDresses = newFlagValue;
        
                DressesFlagChanged?.Invoke();
            }
        }

        public void SetNewHairsFlag(bool newFlagValue)
        {
            if (HaveNewHairs != newFlagValue)
            {
                HaveNewHairs = newFlagValue;
        
                DressesFlagChanged?.Invoke();
            }
        }

        public void SetNewGlassesFlag(bool newFlagValue)
        {
            if (HaveNewGlasses != newFlagValue)
            {
                HaveNewGlasses = newFlagValue;
        
                DressesFlagChanged?.Invoke();
            }
        }
        
        public void SetNewWallsFlag(bool newFlagValue)
        {
            if (HaveNewWalls != newFlagValue)
            {
                HaveNewWalls = newFlagValue;
                
                WallsFlagChanged?.Invoke(HaveNewWalls);
            }
        }
        
        public void SetNewFloorsFlag(bool newFlagValue)
        {
            if (HaveNewFloors != newFlagValue)
            {
                HaveNewFloors = newFlagValue;
        
                FloorsFlagChanged?.Invoke(HaveNewFloors);
            }
        }
    }
}