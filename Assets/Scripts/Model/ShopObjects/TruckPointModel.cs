using System;
using Data;
using UnityEngine;

namespace Model.ShopObjects
{
    public class TruckPointModel : ShopObjectModelBase
    {
        public event Action<int> DeliverTimeUpdated;
        public event Action<int> BoxRemoved;
        
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

        public void RemoveBox(int boxIndex)
        {
            if (boxIndex < ProductsFillList.Length
                && ProductsFillList[boxIndex] > 0)
            {
                ProductsFillList[boxIndex] = 0;
                
                BoxRemoved?.Invoke(boxIndex);
            }
        }

        public ProductType GetProductTypeAtBoxIndex(int boxIndex)
        {
            if (boxIndex < ProductsFillList.Length
                && ProductsFillList[boxIndex] > 0)
            {
                return _setting.Products[boxIndex];
            }

            return ProductType.Undefined;
        }

        public void AdvanceDeliverTime()
        {
            if (DeliverTimeSecondsRest <= 0) return;

            DeliverTimeSecondsRest--;

            DeliverTimeUpdated?.Invoke(DeliverTimeSecondsRest);
        }
    }
}