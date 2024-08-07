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
        
        public readonly ProductType[] ProductSlots;

        public ShelfModel(Vector2Int cellCoords, ShopObjectType shopObjectType, int slotsAmount)
            : base(cellCoords)
        {
            ShopObjectType = shopObjectType;
            UpgradeIndex = 0;

            ProductSlots = Enumerable.Repeat(ProductType.None, slotsAmount).ToArray();
        }

        public override ShopObjectType ShopObjectType { get; }
        public int UpgradeIndex { get; private set; }

        public bool HasEmptySlots()
        {
            return ProductSlots.Any(slot => slot == ProductType.None);
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
    }
}