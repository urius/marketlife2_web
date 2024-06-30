using Data;
using Model.Customers.States;
using UnityEngine;

namespace Events
{
    public class CustomerMovingToExitState : CustomerMovingStateBase
    {
        public CustomerMovingToExitState(Vector2Int targetCell) : base(targetCell)
        {
        }

        public override CustomerGlobalStateName StateName => CustomerGlobalStateName.MovingToExit;
    }
}