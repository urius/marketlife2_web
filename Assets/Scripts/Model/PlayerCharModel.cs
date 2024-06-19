using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Model
{
    public class PlayerCharModel
    {
        public event Action<Vector2Int> CellPositionChanged;
        public event Action ProductsBoxAdded;

        private readonly List<ProductType> _productsInBox = new(Constants.ProductsAmountInBox);
        
        public PlayerCharModel(Vector2Int cellPosition)
        {
            CellPosition = cellPosition;
        }

        public Vector2Int CellPosition { get; private set; }

        public bool HasProducts => _productsInBox.Count > 0;
        public IReadOnlyList<ProductType> ProductsInBox => _productsInBox;

        public void SetCellPosition(Vector2Int cellPosition)
        {
            if (CellPosition == cellPosition) return;
            
            CellPosition = cellPosition;
            CellPositionChanged?.Invoke(CellPosition);
        }

        public void AddProductsBox(IEnumerable<ProductType> products)
        {
            _productsInBox.AddRange(products);
            
            ProductsBoxAdded?.Invoke();
        }
    }
}