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

        public event Action NearShopObjectsUpdated;

        public readonly ProductBoxModel ProductsBox = new();
        
        public PlayerCharModel(Vector2Int cellPosition)
        {
            CellPosition = cellPosition;
        }

        public Vector2Int CellPosition { get; private set; }
        public bool HasProducts => ProductsBox.HasProducts();
        public IReadOnlyList<ProductType> ProductsInBox => ProductsBox.ProductsInBox;

        public CashDeskModel NearCashDesk { get; private set; }
        public ShelfModel NearShelf { get; private set; }
        public TruckPointModel NearTruckPoint { get; private set; }

        public bool IsMultipleShopObjectsNear => (NearCashDesk != null ? 1 : 0) +
            (NearShelf != null ? 1 : 0) +
            (NearTruckPoint != null ? 1 : 0) > 1;

        public void SetCellPosition(Vector2Int cellPosition)
        {
            if (CellPosition == cellPosition) return;

            CellPosition = cellPosition;
            CellPositionChanged?.Invoke(CellPosition);
        }

        public bool CheckPlaySoundDistance(Vector2Int targetCellCoords)
        {
            return Vector2Int.Distance(CellPosition, targetCellCoords) < Constants.CellDistanceToSound;
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

        public void SetNearShopObjects(CashDeskModel nearCashDesk, TruckPointModel nearTruckPoint, ShelfModel nearShelf)
        {
            if (NearCashDesk == nearCashDesk && NearTruckPoint == nearTruckPoint && NearShelf == nearShelf) return;
            
            NearCashDesk = nearCashDesk;
            NearTruckPoint = nearTruckPoint;
            NearShelf = nearShelf;

            NearShopObjectsUpdated?.Invoke();
        }
    }
}