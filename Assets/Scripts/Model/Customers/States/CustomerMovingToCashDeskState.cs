using Data;
using Model.ShopObjects;
using UnityEngine;

namespace Model.Customers.States
{
    public class CustomerMovingToCashDeskState : CustomerMovingStateBase
    {
        public readonly CashDeskModel TargetCashDesk;

        public CustomerMovingToCashDeskState(Vector2Int cashDeskPayPoint, CashDeskModel targetCashDesk) 
            : base(cashDeskPayPoint)
        {
            TargetCashDesk = targetCashDesk;
        }

        public override CustomerGlobalStateName StateName => CustomerGlobalStateName.MovingToCashDesk;
    }
}