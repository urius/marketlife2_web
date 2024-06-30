using Data;
using Model.ShopObjects;
using UnityEngine;

namespace Model.Customers.States
{
    public class CustomerMovingToShelfState : CustomerMovingStateBase
    {
        public readonly ShelfModel TargetShelf;
        public readonly ProductType TargetProduct;

        public CustomerMovingToShelfState(Vector2Int targetCell, ShelfModel targetShelf, ProductType targetProduct) 
            : base(targetCell)
        {
            TargetShelf = targetShelf;
            TargetProduct = targetProduct;
        }

        public override CustomerGlobalStateName StateName => CustomerGlobalStateName.MovingToShelf;
    }
}