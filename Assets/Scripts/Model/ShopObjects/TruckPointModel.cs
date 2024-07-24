using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Model.ShopObjects
{
    public class TruckPointModel : ShopObjectModelBase
    {
        public const int DefaultProductBoxesAmount = 2;
        public const int MaxProductBoxesAmount = 4;
        
        public event Action<int> DeliverTimeAdvanced;
        public event Action DeliverTimeReset;
        public event Action<int> BoxRemoved;
        public event Action Upgraded;
        
        private readonly TruckPointSetting _setting;
        private readonly int[] _deliverTimesByUpgradeIndex;
        
        private int _upgradesCount;
        private int _productBoxesAmount;

        public TruckPointModel(Vector2Int cellCoords,
            TruckPointSetting setting,
            ProductType[] currentProductBoxes,
            int upgradesCount,
            int deliverTimeSecondsRest) : base(cellCoords)
        {
            _setting = setting;
            _upgradesCount = upgradesCount;

            UpdateProductBoxesAmount();
            _deliverTimesByUpgradeIndex = GetDeliverTimes(setting);

            CurrentProductBoxes = currentProductBoxes;
            DeliverTimeSecondsRest = deliverTimeSecondsRest;
        }

        public override ShopObjectType ShopObjectType => ShopObjectType.TruckPoint;

        public ProductType[] CurrentProductBoxes { get; private set; }

        public int DeliverTimeSecondsRest { get; private set; }

        public int UpgradesCount => _upgradesCount;

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

        public void ResetProductsSilently()
        {
            var amountToCopy = Math.Min(_setting.Products.Length, _productBoxesAmount);
            Array.Copy(_setting.Products, CurrentProductBoxes, amountToCopy);
        }

        public bool CanUpgrade()
        {
            return _upgradesCount < _deliverTimesByUpgradeIndex.Length;
        }

        public bool Upgrade()
        {
            var canUpgrade = CanUpgrade();
            
            if (canUpgrade)
            {
                _upgradesCount++;
                UpdateProductBoxesAmount();
                
                Upgraded?.Invoke();
            }

            return canUpgrade;
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

            DeliverTimeAdvanced?.Invoke(DeliverTimeSecondsRest);

            return true;
        }

        public void ResetDeliverTime()
        {
            DeliverTimeSecondsRest = _upgradesCount > 0
                ? _deliverTimesByUpgradeIndex[_upgradesCount - 1]
                : _setting.DeliverTimeSecondsDefault;

            DeliverTimeReset?.Invoke();
        }

        private void UpdateProductBoxesAmount()
        {
            _productBoxesAmount = Math.Min(MaxProductBoxesAmount, DefaultProductBoxesAmount + _upgradesCount);
        }

        private static int[] GetDeliverTimes(TruckPointSetting setting)
        {
            var result = new LinkedList<int>();
            
            var tempDeliverTime = setting.DeliverTimeSecondsDefault;
            var i = 0;
            var productUpgradesAmount = setting.Products.Length - DefaultProductBoxesAmount;
            var upgradeTimeDelta = setting.UpgradesSetting.InitialDeliverTimeUpgradeValue;
            var minDeliverTimeSeconds = setting.UpgradesSetting.MinDeliverTimeSeconds;

            while (i < 100)
            {
                if (i < productUpgradesAmount)
                {
                    result.AddLast(tempDeliverTime);
                }
                else if (tempDeliverTime > minDeliverTimeSeconds)
                {
                    tempDeliverTime -= upgradeTimeDelta;
                    if (upgradeTimeDelta > 1) upgradeTimeDelta--;

                    result.AddLast(tempDeliverTime >= minDeliverTimeSeconds ? tempDeliverTime : minDeliverTimeSeconds);
                }
                else
                {
                    break;
                }

                i++;
            }

            return result.ToArray();
        }
    }
}