using Data;
using UnityEngine;

namespace Model.ShopObjects
{
    public class ShelfModel : ShopObjectModelBase
    {
        public ShelfModel(Vector2Int cellCoords, ShopObjectType shopObjectType) 
            : base(cellCoords)
        {
            ShopObjectType = shopObjectType;
        }

        public override ShopObjectType ShopObjectType { get; }
    }
}