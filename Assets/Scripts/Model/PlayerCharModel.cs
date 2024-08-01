using System;
using System.Collections.Generic;
using Data;
using Model.People;
using Model.ShopObjects;
using UnityEngine;

namespace Model
{
    public class PlayerCharModel
    {
        public event Action<Vector2Int> CellPositionChanged;
        public event Action ProductsBoxAdded
        {
            add => ProductsBox.ProductsBoxAdded += value;
            remove => ProductsBox.ProductsBoxAdded -= value;
        }
        public event Action<int> ProductRemoved
        {
            add => ProductsBox.ProductRemoved += value;
            remove => ProductsBox.ProductRemoved -= value;
        }

        public event Action NearCashDeskUpdated;
        public event Action NearTruckPointUpdated;
        public event Action NearShelfUpdated;

        public PlayerCharModel(Vector2Int cellPosition)
        {
            CellPosition = cellPosition;
        }

        public Vector2Int CellPosition { get; private set; }
        public bool HasProducts => ProductsBox.HasProducts();
        public IReadOnlyList<ProductType> ProductsInBox => ProductsBox.ProductsInBox;
        public ProductBoxModel ProductsBox { get; } = new();

        public CashDeskModel NearCashDesk { get; private set; }
        public ShelfModel NearShelf { get; private set; }
        public TruckPointModel NearTruckPoint { get; private set; }

        public void SetCellPosition(Vector2Int cellPosition)
        {
            if (CellPosition == cellPosition) return;

            CellPosition = cellPosition;
            CellPositionChanged?.Invoke(CellPosition);
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

        public void FillBoxWithProduct(ProductType product)
        {
            ProductsBox.FillBoxWithProduct(product);
        }

        public int GetNextNotEmptySlotIndex()
        {
            return ProductsBox.GetNextNotEmptySlotIndex();
        }

        public void RemoveProductFromSlot(int boxSlotIndex)
        {
            ProductsBox.RemoveProductFromSlot(boxSlotIndex);
        }
    }
}