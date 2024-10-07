using System;
using System.Linq;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Model.ShopObjects
{
    public class ShelfModel : ShopObjectModelBase
    {
        public event Action<int> ProductAdded;
        public event Action<int> ProductRemoved;
        public event Action<int> UpgradeIndexChanged;

        public ShelfModel(
            Vector2Int cellCoords, 
            ShopObjectType shopObjectType, 
            int slotsAmount,
            int upgradeIndex = 0,
            ProductType[] products = null)
            : base(cellCoords)
        {
            ShopObjectType = shopObjectType;
            UpgradeIndex = upgradeIndex;

            ProductSlots = products ?? Enumerable.Repeat(ProductType.None, slotsAmount).ToArray();
        }

        public override ShopObjectType ShopObjectType { get; }
        public int UpgradeIndex { get; private set; }
        
        public ProductType[] ProductSlots { get; private set; }

        public void IncrementUpgradeIndex()
        {
            UpgradeIndex++;
            
            UpgradeIndexChanged?.Invoke(UpgradeIndex);
        }

        public bool HasEmptySlots()
        {
            foreach (var slot in ProductSlots)
            {
                if (slot == ProductType.None)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasProducts()
        {
            foreach (var product in ProductSlots)
            {
                if (product != ProductType.None)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasProduct(ProductType productType)
        {
            return ProductSlots.Any(slot => slot == productType);
        }
        
        public int GetProductSlotIndex(ProductType productType)
        {
            return Array.IndexOf(ProductSlots, productType);
        }
        
        public ProductType GetRandomNotEmptyProductOrDefault()
        {
            var nonEmptySlots = ProductSlots
                .Where(slot => slot != ProductType.None)
                .ToArray();
    
            if (nonEmptySlots.Length > 0)
            {
                var randomIndex = Random.Range(0, nonEmptySlots.Length);
                
                return nonEmptySlots[randomIndex];
            }
    
            return ProductType.None;
        }
        
        public int GetEmptySlotIndex()
        {
            for (var i = 0; i < ProductSlots.Length; i++)
            {
                if (ProductSlots[i] == ProductType.None)
                {
                    return i;
                }
            }
    
            return -1;
        }
        
        public void AddProductToSlot(int slotIndex, ProductType productType)
        {
            if (slotIndex < ProductSlots.Length)
            {
                ProductSlots[slotIndex] = productType;
                
                ProductAdded?.Invoke(slotIndex);
            }
        }

        public void RemoveProductFromSlotIndex(int slotIndex)
        {
            if (slotIndex < ProductSlots.Length)
            {
                ProductSlots[slotIndex] = ProductType.None;
                
                ProductRemoved?.Invoke(slotIndex);
            }
        }

        public void SetSlotsAmount(int slotsAmount)
        {
            var prevProducts = ProductSlots;
            
            ProductSlots = Enumerable.Repeat(ProductType.None, slotsAmount).ToArray();
            
            Array.Copy(prevProducts, ProductSlots, prevProducts.Length);
        }
    }
}