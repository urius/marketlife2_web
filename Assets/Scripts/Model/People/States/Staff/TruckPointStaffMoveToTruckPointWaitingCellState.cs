using Data;
using Model.ShopObjects;
using UnityEngine;

namespace Model.People.States.Staff
{
    public class TruckPointStaffMoveToTruckPointWaitingCellState : BotCharMovingStateBase
    {
        public readonly TruckPointModel TruckPointModel;

        public TruckPointStaffMoveToTruckPointWaitingCellState(TruckPointModel truckPointModel, Vector2Int targetCell)
            : base(targetCell)
        {
            TruckPointModel = truckPointModel;
        }

        public override ShopCharStateName StateName => ShopCharStateName.TpStaffMovingToTruckPointWaitingCell;
    }
}