using Data;
using Model.ShopObjects;
using UnityEngine;

namespace Model.People.States.Customer
{
    public class CustomerMovingToShelfState : BotCharMovingStateBase
    {
        public readonly ShelfModel TargetShelf;
        public readonly ProductType TargetProduct;

        public CustomerMovingToShelfState(Vector2Int targetCell, ShelfModel targetShelf, ProductType targetProduct) 
            : base(targetCell)
        {
            TargetShelf = targetShelf;
            TargetProduct = targetProduct;
        }

        public override ShopCharStateName StateName => ShopCharStateName.CustomerMovingToShelf;
    }
}