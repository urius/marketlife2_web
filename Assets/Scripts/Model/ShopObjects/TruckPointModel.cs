using Data;
using UnityEngine;

namespace Model.ShopObjects
{
    public class TruckPointModel : ShopObjectModelBase
    {
        public TruckPointModel(Vector2Int cellCoords) : base(cellCoords)
        {
        }

        public override ShopObjectType ShopObjectType => ShopObjectType.TruckPoint;
        public long ReadyTimestamp { get; private set; } = 0;
    }
}