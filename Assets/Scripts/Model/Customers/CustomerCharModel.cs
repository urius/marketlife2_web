using System;
using System.Collections.Generic;
using Data;
using Events;
using Model.Customers.States;
using Model.ShopObjects;
using UnityEngine;

namespace Model.Customers
{
    public class CustomerCharModel
    {
        private const int MaxProductsAmount = 4;
        
        public event Action<Vector2Int> CellPositionChanged;
        public event Action<int> ProductAdded;
        public event Action<CustomerStateBase> StateChanged;

        public bool IsStepInProgress = false;
        
        private readonly List<ProductType> _products = new(MaxProductsAmount);
        
        private Vector2Int _prevCellPosition;
        private Vector2Int _cellPosition;
        private int _stepIndex;

        public CustomerCharModel(Vector2Int cellPosition)
        {
            CellPosition = cellPosition;
        }

        public int ProductsCount => _products.Count;
        public bool HasProducts => ProductsCount > 0;
        public Vector2Int PreviousCellPosition => _prevCellPosition;
        public Vector2Int CellPosition
        {
            get => _cellPosition;
            private set
            {
                _prevCellPosition = _cellPosition;
                _cellPosition = value;
                CellPositionChanged?.Invoke(value);
            }
        }

        public CustomerStateBase State { get; private set; }


        public void SetCellPosition(Vector2Int stepCell)
        {
            CellPosition = stepCell;
        }

        public void SetTakingProductState(ShelfModel targetShelfModel, int slotIndex, ProductType productType,
            int addedProductSlotIndex)
        {
            SetState(new CustomerTakeProductFromShelfState(targetShelfModel, slotIndex, productType, addedProductSlotIndex));
        }
        
        public void SetMovingToEnterState(Vector2Int targetCell)
        {
            SetState(new CustomerMovingToEnterState(targetCell));
        }
        
        public void SetMovingToExitState(Vector2Int targetCell)
        {
            SetState(new CustomerMovingToExitState(targetCell));
        }

        public void SetMovingToDespawnState(Vector2Int despawnPoint)
        {
            SetState(new CustomerMovingToDespawnState(despawnPoint));
        }

        public void SetMovingToShelfState(Vector2Int targetCell, ShelfModel targetShelf, ProductType targetProduct)
        {
            SetState(new CustomerMovingToShelfState(targetCell, targetShelf, targetProduct));
        }

        public void SetMovingToCashDeskState(CashDeskModel targetCashDesk, Vector2Int cashDeskPayPoint)
        {
            SetState(new CustomerMovingToCashDeskState(cashDeskPayPoint, targetCashDesk));
        }

        public void SetPayingState(CashDeskModel targetCashDesk)
        {
            SetState(new CustomerPayingState(targetCashDesk));
        }

        private void SetState(CustomerStateBase state)
        {
            State = state;

            StateChanged?.Invoke(State);
        }

        public int AddProduct(ProductType productType)
        {
            _products.Add(productType);

            var addedProductSlotIndex = _products.Count - 1;
            
            ProductAdded?.Invoke(addedProductSlotIndex);

            return addedProductSlotIndex;
        }

        public ProductType GetProductTypeAtSlot(int slotIndex)
        {
            if (slotIndex < _products.Count)
            {
                return _products[slotIndex];
            }

            return ProductType.None;
        }
    }
}