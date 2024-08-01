using Data;
using Model.ShopObjects;
using UnityEngine;

namespace Model.People.States.Staff
{
    public class TruckPointStaffMoveToTruckPointState : BotCharMovingStateBase
    {
        public readonly TruckPointModel TargetTruckPointModel;

        public TruckPointStaffMoveToTruckPointState(TruckPointModel targetTruckPointModel, Vector2Int targetCell) 
            : base(targetCell)
        {
            TargetTruckPointModel = targetTruckPointModel;
        }

        public override ShopCharStateName StateName => ShopCharStateName.TpStaffMovingToTruckPoint;
    }
}