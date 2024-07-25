using Data;
using Model.ShopObjects;

namespace Model.People.States.Customer
{
    public class CustomerPayingState : ShopSharStateBase
    {
        public readonly CashDeskModel TargetCashDesk;

        public CustomerPayingState(CashDeskModel targetCashDesk)
        {
            TargetCashDesk = targetCashDesk;
        }

        public override ShopCharStateName StateName => ShopCharStateName.Paying;
    }
}