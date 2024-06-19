using System;
using System.Linq;
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
            ProductType[] currentProductBoxes,
            int deliverTimeSecondsRest) : base(cellCoords)
        {
            _setting = setting;
            
            CurrentProductBoxes = currentProductBoxes;
            DeliverTimeSecondsRest = deliverTimeSecondsRest;
        }

        public override ShopObjectType ShopObjectType => ShopObjectType.TruckPoint;
        public ProductType[] CurrentProductBoxes { get; private set; }
        public int DeliverTimeSecondsRest { get; private set; }
        public bool HasProducts => CurrentProductBoxes.Any(p => p != ProductType.None);

        public void RemoveBox(int boxIndex)
        {
            if (boxIndex < CurrentProductBoxes.Length
                && CurrentProductBoxes[boxIndex] != ProductType.None)
            {
                CurrentProductBoxes[boxIndex] = ProductType.None;

                BoxRemoved?.Invoke(boxIndex);
            }
        }

        public ProductType GetProductTypeAtBoxIndex(int boxIndex)
        {
            return boxIndex < CurrentProductBoxes.Length ? CurrentProductBoxes[boxIndex] : ProductType.None;
        }

        public int GetFirstNotEmptyProductBoxIndex()
        {
            for (var i = 0; i < CurrentProductBoxes.Length; i++)
            {
                var currentProductBox = CurrentProductBoxes[i];

                if (currentProductBox != ProductType.None)
                {
                    return i;
                }
            }

            return -1;
        }

        public bool AdvanceDeliverTime()
        {
            if (DeliverTimeSecondsRest <= 0) return false;

            DeliverTimeSecondsRest--;

            DeliverTimeUpdated?.Invoke(DeliverTimeSecondsRest);

            return true;
        }
    }
}