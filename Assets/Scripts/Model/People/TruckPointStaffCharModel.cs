using System;
using System.Collections.Generic;
using Data;
using Model.People.States.Staff;
using Model.ShopObjects;
using UnityEngine;

namespace Model.People
{
    public class TruckPointStaffCharModel : StaffCharModelBase
    {
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

        public TruckPointStaffCharModel(
            Vector2Int cellCoords, 
            int workSecondsLeft, 
            IReadOnlyList<ProductType> products) 
            : base(cellCoords, workSecondsLeft)
        {

            for (var i = 0; i < ProductsBox.ProductsInBox.Length; i++)
            {
                if (i < products.Count)
                {
                    ProductsBox.ProductsInBox[i] = products[i];
                }
            }
        }

        public TruckPointStaffCharModel(
            Vector2Int cellCoords,
            int workSecondsLeft)
            : this(
                cellCoords,
                workSecondsLeft,
                Array.Empty<ProductType>())
        {
        }

        public IReadOnlyList<ProductType> ProductsInBox => ProductsBox.ProductsInBox;
        public bool HasProducts => ProductsBox.HasProducts();
        public ProductBoxModel ProductsBox { get; } = new();

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

        public void SetMoveToTruckPointState(TruckPointModel targetTruckPointModel, Vector2Int targetCell)
        {
            SetState(new TruckPointStaffMoveToTruckPointState(targetTruckPointModel, targetCell));
        }

        public void SetMoveToTruckPointWaitingCellState(TruckPointModel truckPointModel, Vector2Int targetCell)
        {
            SetState(new TruckPointStaffMoveToTruckPointWaitingCellState(truckPointModel, targetCell));
        }

        public void SetTakeProductFromTruckPointState(TruckPointModel truckPointModel, int productBoxIndexToTake)
        {
            SetState(new StaffTakeProductFromTruckPointState(truckPointModel, productBoxIndexToTake));
        }

        public void SetMovingToShelfState(Vector2Int targetCell, ShelfModel targetShelf)
        {
            SetState(new TruckPointStaffMovingToShelfState(targetCell, targetShelf));
        }

        public void SetPutProductsToShelfState(ShelfModel targetShelf)
        {
            SetState(new TruckPointStaffPutProductsOnShelfState(targetShelf));
        }

        public void SetIdleState()
        {
            SetState(StaffIdleState.Instance);
        }
    }
}