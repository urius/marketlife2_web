using Data;
using UnityEngine;

namespace Model.Customers.States
{
    public class CustomerMovingToDespawnState : CustomerMovingStateBase
    {
        public CustomerMovingToDespawnState(Vector2Int targetCell) : base(targetCell)
        {
        }

        public override CustomerGlobalStateName StateName => CustomerGlobalStateName.MovingToDespawn;
    }
}