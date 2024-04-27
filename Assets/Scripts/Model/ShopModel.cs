using Data;
using UnityEngine;

namespace Model
{
    public class ShopModel
    {
        public Vector2Int Size;
        public WallType WallsType;

        public ShopModel(Vector2Int size, WallType wallsType)
        {
            Size = size;
            WallsType = wallsType;
        }
    }
}