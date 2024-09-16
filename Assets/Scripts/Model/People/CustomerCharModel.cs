using System;
using System.Collections.Generic;
using Data;
using Events;
using Model.People.States.Customer;
using Model.ShopObjects;
using UnityEngine;

namespace Model.People
{
    public class CustomerCharModel : BotCharModelBase
    {
        public const int MaxProductsAmount = 4;
        
        public event Action<int> ProductAdded;
        public event Action BagStatusUpdated;
        
        private readonly List<ProductType> _products = new(MaxProductsAmount);

        public CustomerCharModel(Vector2Int cellPosition) 
            : base(cellPosition)
        {
        }

        public bool HasBag { get; private set; }
        public int ProductsCount => _products.Count;
        public bool HasProducts => ProductsCount > 0;

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

        public void EnableBag()
        {
            HasBag = true;
            BagStatusUpdated?.Invoke();
        }
    }
}