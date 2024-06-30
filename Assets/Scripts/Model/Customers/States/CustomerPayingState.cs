using Data;
using Model.ShopObjects;

namespace Model.Customers.States
{
    public class CustomerPayingState : CustomerStateBase
    {
        public readonly CashDeskModel TargetCashDesk;

        public CustomerPayingState(CashDeskModel targetCashDesk)
        {
            TargetCashDesk = targetCashDesk;
        }

        public override CustomerGlobalStateName StateName => CustomerGlobalStateName.Paying;
    }
}