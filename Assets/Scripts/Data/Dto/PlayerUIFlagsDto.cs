using System;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerUIFlagsDto
    {
        public bool HaveNewWalls;
        public bool HaveNewFloors;

        public PlayerUIFlagsDto(bool haveNewWalls, bool haveNewFloors)
        {
            HaveNewWalls = haveNewWalls;
            HaveNewFloors = haveNewFloors;
        }
    }
}