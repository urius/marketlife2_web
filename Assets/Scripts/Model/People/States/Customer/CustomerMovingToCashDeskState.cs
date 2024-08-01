using Data;
using Model.ShopObjects;
using UnityEngine;

namespace Model.People.States.Customer
{
    public class CustomerMovingToCashDeskState : BotCharMovingStateBase
    {
        public readonly CashDeskModel TargetCashDesk;

        public CustomerMovingToCashDeskState(Vector2Int cashDeskPayPoint, CashDeskModel targetCashDesk) 
            : base(cashDeskPayPoint)
        {
            TargetCashDesk = targetCashDesk;
        }

        public override ShopCharStateName StateName => ShopCharStateName.CustomerMovingToCashDesk;
    }
}