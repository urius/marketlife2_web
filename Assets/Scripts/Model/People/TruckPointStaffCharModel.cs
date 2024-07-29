using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Model.People
{
    public class TruckPointStaffCharModel : StaffCharModel
    {
        private readonly ProductType[] _productsInBox =
            Enumerable.Repeat(ProductType.None, Constants.ProductsAmountInBox).ToArray();
        
        public TruckPointStaffCharModel(Vector2Int cellCoords, int workSecondsLeft, IReadOnlyList<ProductType> products) 
            : base(cellCoords, workSecondsLeft)
        {

            for (var i = 0; i < _productsInBox.Length; i++)
            {
                if (i < products.Count)
                {
                    _productsInBox[i] = products[i];
                }
            }
        }
        
        public IReadOnlyList<ProductType> ProductsInBox => _productsInBox;
        public bool HasProducts => HasProductsInternal();
        
        private bool HasProductsInternal()
        {
            foreach (var product in _productsInBox)
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