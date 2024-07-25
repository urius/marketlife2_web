using Data;
using UnityEngine;

namespace Model.People.States.Customer
{
    public class CustomerMovingToDespawnState : CustomerMovingStateBase
    {
        public CustomerMovingToDespawnState(Vector2Int targetCell) : base(targetCell)
        {
        }

        public override ShopCharStateName StateName => ShopCharStateName.MovingToDespawn;
    }
}