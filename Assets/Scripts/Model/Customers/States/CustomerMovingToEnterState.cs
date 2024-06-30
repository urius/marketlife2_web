using Data;
using UnityEngine;

namespace Model.Customers.States
{
    public class CustomerMovingToEnterState : CustomerMovingStateBase
    {
        public CustomerMovingToEnterState(Vector2Int targetCell) : base(targetCell)
        {
        }

        public override CustomerGlobalStateName StateName => CustomerGlobalStateName.MovingToEnter;
    }
}