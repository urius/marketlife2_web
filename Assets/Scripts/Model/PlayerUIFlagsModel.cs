using System;

namespace Model
{
    public class PlayerUIFlagsModel
    {
        public event Action<bool> WallsFlagChanged;
        public event Action<bool> FloorsFlagChanged;

        public PlayerUIFlagsModel(bool haveNewWalls, bool haveNewFloors)
        {
            HaveNewWalls = haveNewWalls;
            HaveNewFloors = haveNewFloors;
        }
        
        public bool HaveNewWalls { get; private set; }
        public bool HaveNewFloors { get; private set; }
        
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