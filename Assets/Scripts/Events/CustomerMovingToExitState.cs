using Data;
using Model.People.States;
using Model.People.States.Customer;
using UnityEngine;

namespace Events
{
    public class CustomerMovingToExitState : CustomerMovingStateBase
    {
        public CustomerMovingToExitState(Vector2Int targetCell) : base(targetCell)
        {
        }

        public override ShopCharStateName StateName => ShopCharStateName.MovingToExit;
    }
}