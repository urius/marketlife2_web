using Model.ShopObjects;

namespace Events
{
    public struct AnimateTakeMoneyFromCashDeskEvent
    {
        public readonly CashDeskModel TargetCashDesk;
        public readonly int MoneyAmount;

        public AnimateTakeMoneyFromCashDeskEvent(CashDeskModel targetCashDesk, int moneyAmount)
        {
            TargetCashDesk = targetCashDesk;
            MoneyAmount = moneyAmount;
        }
    }
}