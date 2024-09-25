using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Model.People;
using UnityEngine;

namespace Model.ShopObjects
{
    public class TruckPointModel : ShopObjectModelBase
    {
        public const int DefaultProductBoxesAmount = 2;
        public const int MaxProductBoxesAmount = 4;
        
        public event Action<int> DeliverTimeAdvanced;
        public event Action DeliverTimeReset;
        public event Action ProductsReset;
        public event Action<int> BoxRemoved;
        public event Action Upgraded;
        public event Action<TruckPointStaffCharModel> StaffAdded;
        public event Action<TruckPointStaffCharModel> StaffRemoved;
        
        private readonly TruckPointSetting _setting;
        private readonly int[] _deliverTimesByUpgradeIndex;
        private readonly ProductType[] _currentProductBoxes;
        
        private int _upgradesCount;
        private int _productBoxesAmount;

        public TruckPointModel(Vector2Int cellCoords,
            TruckPointSetting setting,
            ProductType[] currentProductBoxes,
            int upgradesCount,
            int deliverTimeSecondsRest,
            TruckPointStaffCharModel staffModel = null) : base(cellCoords)
        {
            _setting = setting;
            _upgradesCount = upgradesCount;

            UpdateProductBoxesAmount();
            _deliverTimesByUpgradeIndex = GetDeliverTimes(setting);

            _currentProductBoxes = new ProductType[_setting.Products.Length];
            Array.Copy(currentProductBoxes, _currentProductBoxes, currentProductBoxes.Length);
            
            DeliverTimeSecondsRest = deliverTimeSecondsRest;
            StaffCharModel = staffModel;
        }

        public override ShopObjectType ShopObjectType => ShopObjectType.TruckPoint;
        public int DeliverTimeSecondsRest { get; private set; }
        public TruckPointStaffCharModel StaffCharModel { get; private set; }
        public bool IsDelivered => DeliverTimeSecondsRest <= 0 && HasProducts;
        public int UpgradesCount => _upgradesCount;
        public bool HasProducts => HasProductsInternal();
        public bool HasStaff => StaffCharModel != null;

        public void RemoveBox(int boxIndex)
        {
            if (boxIndex < _currentProductBoxes.Length
                && _currentProductBoxes[boxIndex] != ProductType.None)
            {
                _currentProductBoxes[boxIndex] = ProductType.None;

                BoxRemoved?.Invoke(boxIndex);
            }
        }

        public void ResetProducts()
        {
            var amountToCopy = Math.Min(_setting.Products.Length, _productBoxesAmount);
            Array.Copy(_setting.Products, _currentProductBoxes, amountToCopy);
            
            ProductsReset?.Invoke();
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

        public void AddStaff(TruckPointStaffCharModel staffCharModel)
        {
            SetStaffModel(staffCharModel);
        }
        
        public void RemoveStaff()
        {
            SetStaffModel(null);
        }

        public ProductType GetProductTypeAtBoxIndex(int boxIndex)
        {
            return boxIndex < _currentProductBoxes.Length ? _currentProductBoxes[boxIndex] : ProductType.None;
        }

        public int GetFirstNotEmptyProductBoxIndex()
        {
            for (var i = 0; i < _currentProductBoxes.Length; i++)
            {
                var currentProductBox = _currentProductBoxes[i];

                if (currentProductBox != ProductType.None)
                {
                    return i;
                }
            }

            return -1;
        }

        public int GetLastNotEmptyProductBoxIndex()
        {
            for (var i = _currentProductBoxes.Length - 1; i >= 0; i--)
            {
                var currentProductBox = _currentProductBoxes[i];

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
            DeliverTimeSecondsRest = GetDeliverTimeSettingSeconds();

            DeliverTimeReset?.Invoke();
        }

        public int GetDeliverTimeSettingSeconds()
        {
            return _upgradesCount > 0
                ? _deliverTimesByUpgradeIndex[_upgradesCount - 1]
                : _setting.DeliverTimeSecondsDefault;
        }
        
        public ProductType[] GetAvailableProducts()
        {
            return _setting.Products.Take(_productBoxesAmount).ToArray();
        }

        private void SetStaffModel(TruckPointStaffCharModel cashDeskStaffModel)
        {
            var prevStaffModel = StaffCharModel;
            
            StaffCharModel = cashDeskStaffModel;

            if (StaffCharModel != null)
            {
                StaffAdded?.Invoke(StaffCharModel);
            }
            else
            {
                StaffRemoved?.Invoke(prevStaffModel);
            }
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
            var upgradeTimeDelta = setting.InitialDeliverTimeUpgradeValue;
            const int minDeliverTimeSeconds = Constants.MinDeliverTimeSeconds;

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

        private bool HasProductsInternal()
        {
            foreach (var productBox in _currentProductBoxes)
            {
                if (productBox != ProductType.None)
                {
                    return true;
                }
            }

            return false;
        }
    }
}