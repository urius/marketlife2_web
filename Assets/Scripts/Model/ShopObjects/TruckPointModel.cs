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
        public event Action<int> StaffAdded;
        public event Action<int> StaffRemoved;
        
        private readonly TruckPointSetting _setting;
        private readonly int[] _deliverTimesByUpgradeIndex;
        private readonly ProductType[] _currentProductBoxes;
        private readonly TruckPointStaffCharModel[] _staffCharModels = new TruckPointStaffCharModel[2];
        
        private int _upgradesCount;
        private int _productBoxesAmount;

        public TruckPointModel(Vector2Int cellCoords,
            TruckPointSetting setting,
            ProductType[] currentProductBoxes,
            int upgradesCount,
            int deliverTimeSecondsRest,
            IReadOnlyList<TruckPointStaffCharModel> staffModels) : base(cellCoords)
        {
            _setting = setting;
            _upgradesCount = upgradesCount;

            UpdateProductBoxesAmount();
            _deliverTimesByUpgradeIndex = GetDeliverTimes(setting);

            _currentProductBoxes = new ProductType[_setting.Products.Length];
            Array.Copy(currentProductBoxes, _currentProductBoxes, currentProductBoxes.Length);
            
            DeliverTimeSecondsRest = deliverTimeSecondsRest;

            for (var i = 0; i < _staffCharModels.Length; i++)
            {
                if (i < staffModels.Count)
                {
                    _staffCharModels[i] = staffModels[i];
                }
            }
        }

        public override ShopObjectType ShopObjectType => ShopObjectType.TruckPoint;
        public int DeliverTimeSecondsRest { get; private set; }
        public int UpgradesCount => _upgradesCount;
        public bool HasProducts => _currentProductBoxes.Any(p => p != ProductType.None);
        public IReadOnlyList<TruckPointStaffCharModel> StaffCharModels => _staffCharModels;

        public int GetHiredStaffAmount()
        {
            return _staffCharModels.Count(m => m != null);
        }

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

        public int AddStaffToFreeSlot(TruckPointStaffCharModel staffCharModel)
        {
            var slotIndex = GetReadyToHireStaffSlotIndex();
            
            if (slotIndex >= 0)
            {
                SetStaffModel(slotIndex, staffCharModel);
            }

            return slotIndex;
        }
        
        public bool RemoveStaff(TruckPointStaffCharModel staffCharModel)
        {
            for (var i = 0; i < _staffCharModels.Length; i++)
            {
                if (_staffCharModels[i] == staffCharModel)
                {
                    SetStaffModel(i, null);
                    return true;
                }
            }

            return false;
        }

        public bool CanAddStaff()
        {
            return GetReadyToHireStaffSlotIndex() >= 0;
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

        private int GetReadyToHireStaffSlotIndex()
        {
            for (var i = 0; i < _staffCharModels.Length; i++)
            {
                if (_staffCharModels[i] == null || _staffCharModels[i].WorkSecondsLeft <= 0)
                {
                    return i;
                }
            }

            return -1;
        }
        
        private void SetStaffModel(int slotIndex, TruckPointStaffCharModel staffCharModel)
        {
            _staffCharModels[slotIndex] = staffCharModel;

            if (staffCharModel != null)
            {
                StaffAdded?.Invoke(slotIndex);
            }
            else
            {
                StaffRemoved?.Invoke(slotIndex);
            }
        }
    }
}