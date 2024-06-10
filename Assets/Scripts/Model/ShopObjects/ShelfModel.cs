using Data;
using UnityEngine;

namespace Model.ShopObjects
{
    public class ShelfModel : ShopObjectModelBase
    {
        private int _slotsAmount;

        public ShelfModel(Vector2Int cellCoords, ShopObjectType shopObjectType, int slotsAmount) 
            : base(cellCoords)
        {
            ShopObjectType = shopObjectType;
            UpgradeIndex = 0;
            
            _slotsAmount = slotsAmount;
        }

        public override ShopObjectType ShopObjectType { get; }
        public int UpgradeIndex { get; private set; }
    }
}