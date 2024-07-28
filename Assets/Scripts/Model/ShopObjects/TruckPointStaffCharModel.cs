using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Model.ShopObjects
{
    public class TruckPointStaffCharModel
    {
        private readonly ProductType[] _productsInBox =
            Enumerable.Repeat(ProductType.None, Constants.ProductsAmountInBox).ToArray();
        
        public TruckPointStaffCharModel(int workSecondsLeft, Vector2Int cellCoords, IReadOnlyList<ProductType> products)
        {
            WorkSecondsLeft = workSecondsLeft;
            CellCoords = cellCoords;

            for (var i = 0; i < _productsInBox.Length; i++)
            {
                if (i < products.Count)
                {
                    _productsInBox[i] = products[i];
                }
            }
        }
        
        public IReadOnlyList<ProductType> ProductsInBox => _productsInBox;
        public int WorkSecondsLeft { get; private set; }
        public Vector2Int CellCoords { get; private set; }
    }
}