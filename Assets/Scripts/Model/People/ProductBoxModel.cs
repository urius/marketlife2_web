using System;
using System.Collections.Generic;
using System.Linq;
using Data;

namespace Model.People
{
    public class ProductBoxModel
    {
        public event Action ProductsBoxAdded;
        public event Action<int> ProductRemoved;
        
        public readonly ProductType[] ProductsInBox;

        public ProductBoxModel(IReadOnlyList<ProductType> productsInBox)
        {
            ProductsInBox = Enumerable.Repeat(ProductType.None, Constants.ProductsAmountInBox).ToArray();
            
            if (productsInBox != null)
            {
                for (var i = 0; i < productsInBox.Count; i++)
                {
                    if (i < ProductsInBox.Length)
                    {
                        ProductsInBox[i] = productsInBox[i];
                    }
                }
            }
        }
        
        public void FillBoxWithProduct(ProductType product)
        {
            for (var i = 0; i < ProductsInBox.Length; i++)
            {
                ProductsInBox[i] = product;
            }

            ProductsBoxAdded?.Invoke();
        }

        public void RemoveProductFromSlot(int slotIndex)
        {
            if (slotIndex < ProductsInBox.Length)
            {
                ProductsInBox[slotIndex] = ProductType.None;

                ProductRemoved?.Invoke(slotIndex);
            }
        }

        public int GetNextNotEmptySlotIndex()
        {
            for (var i = 0; i < ProductsInBox.Length; i++)
            {
                if (ProductsInBox[i] != ProductType.None)
                {
                    return i;
                }
            }

            return -1;
        }
        
        public bool HasProducts()
        {
            foreach (var product in ProductsInBox)
            {
                if (product != ProductType.None)
                {
                    return true;
                }
            }

            return false;
        }
    }
}