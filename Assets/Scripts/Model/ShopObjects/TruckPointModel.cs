using System;
using Data;
using UnityEngine;

namespace Model.ShopObjects
{
    public class TruckPointModel : ShopObjectModelBase
    {
        public event Action<int> DelivarTimeUpdated;
        
        private readonly TruckPointSetting _setting;

        public TruckPointModel(Vector2Int cellCoords,
            TruckPointSetting setting,
            int[] productsFillList,
            int deliverTimeSecondsRest) : base(cellCoords)
        {
            _setting = setting;
            
            ProductsFillList = productsFillList;
            DeliverTimeSecondsRest = deliverTimeSecondsRest;
        }

        public override ShopObjectType ShopObjectType => ShopObjectType.TruckPoint;
        public int[] ProductsFillList { get; private set; }
        public int DeliverTimeSecondsRest { get; private set; }

        public void AdvanceDeliverTime()
        {
            if (DeliverTimeSecondsRest <= 0) return;

            DeliverTimeSecondsRest--;

            DelivarTimeUpdated?.Invoke(DeliverTimeSecondsRest);
        }
    }
}