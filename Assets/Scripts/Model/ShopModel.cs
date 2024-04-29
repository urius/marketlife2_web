using Data;
using UnityEngine;

namespace Model
{
    public class ShopModel
    {
        public Vector2Int Size;
        public WallType WallsType;
        public FloorType FloorsType;

        public ShopModel(Vector2Int size, WallType wallsType, FloorType floorsType)
        {
            Size = size;
            WallsType = wallsType;
            FloorsType = floorsType;
        }
    }
}