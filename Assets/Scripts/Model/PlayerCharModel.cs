using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Model.ShopObjects;
using UnityEngine;

namespace Model
{
    public class PlayerCharModel
    {
        public event Action<Vector2Int> CellPositionChanged;
        public event Action ProductsBoxAdded;
        public event Action<int> ProductRemoved;
        public event Action NearCashDeskUpdated;
        public event Action NearTruckPointUpdated;
        public event Action NearShelfUpdated;

        private readonly ProductType[] _productsInBox =
            Enumerable.Repeat(ProductType.None, Constants.ProductsAmountInBox).ToArray();

        public PlayerCharModel(Vector2Int cellPosition)
        {
            CellPosition = cellPosition;
        }

        public Vector2Int CellPosition { get; private set; }
        public bool HasProducts => HasProductsInternal();
        public IReadOnlyList<ProductType> ProductsInBox => _productsInBox;
        public CashDeskModel NearCashDesk { get; private set; }
        public ShelfModel NearShelf { get; private set; }
        public TruckPointModel NearTruckPoint { get; private set; }

        public void SetCellPosition(Vector2Int cellPosition)
        {
            if (CellPosition == cellPosition) return;

            CellPosition = cellPosition;
            CellPositionChanged?.Invoke(CellPosition);
        }

        public void SetProductsInBox(ProductType[] products)
        {
            for (var i = 0; i < _productsInBox.Length; i++)
            {
                if (i < products.Length)
                {
                    _productsInBox[i] = products[i];
                }
            }

            ProductsBoxAdded?.Invoke();
        }

        public void RemoveProductFromSlot(int slotIndex)
        {
            if (slotIndex < _productsInBox.Length)
            {
                _productsInBox[slotIndex] = ProductType.None;

                ProductRemoved?.Invoke(slotIndex);
            }
        }

        public int GetNextNotEmptySlotIndex()
        {
            for (var i = 0; i < _productsInBox.Length; i++)
            {
                if (_productsInBox[i] != ProductType.None)
                {
                    return i;
                }
            }

            return -1;
        }

        public void SetNearCashDesk(CashDeskModel cashDeskModel)
        {
            if (NearCashDesk == cashDeskModel) return;

            NearCashDesk = cashDeskModel;
            NearCashDeskUpdated?.Invoke();
        }

        public void SetNearShelf(ShelfModel shelfModel)
        {
            if (NearShelf == shelfModel) return;

            NearShelf = shelfModel;
            NearShelfUpdated?.Invoke();
        }

        public void SetNearTruckPoint(TruckPointModel truckPointModel)
        {
            if (NearTruckPoint == truckPointModel) return;

            NearTruckPoint = truckPointModel;
            NearTruckPointUpdated?.Invoke();
        }

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