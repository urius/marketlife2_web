using Data;
using Model.ShopObjects;
using UnityEngine;

namespace Model.People.States.Staff
{
    public class TruckPointStaffMovingToShelfState : BotCharMovingStateBase
    {
        public readonly ShelfModel TargetShelf;

        public TruckPointStaffMovingToShelfState(Vector2Int targetCell, ShelfModel targetShelf) : base(targetCell)
        {
            TargetShelf = targetShelf;
        }

        public override ShopCharStateName StateName => ShopCharStateName.TpStaffMovingToShelf;
    }
}