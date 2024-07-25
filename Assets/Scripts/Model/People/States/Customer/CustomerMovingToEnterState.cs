using Data;
using UnityEngine;

namespace Model.People.States.Customer
{
    public class CustomerMovingToEnterState : CustomerMovingStateBase
    {
        public CustomerMovingToEnterState(Vector2Int targetCell) : base(targetCell)
        {
        }

        public override ShopCharStateName StateName => ShopCharStateName.MovingToEnter;
    }
}