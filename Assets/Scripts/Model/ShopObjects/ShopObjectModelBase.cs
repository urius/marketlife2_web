using Data;
using UnityEngine;

namespace Model.ShopObjects
{
    public abstract class ShopObjectModelBase
    {
        protected ShopObjectModelBase(Vector2Int cellCoords)
        {
            CellCoords = cellCoords;
        }

        public abstract ShopObjectType ShopObjectType { get; }
        
        public Vector2Int CellCoords { get; }
    }
}