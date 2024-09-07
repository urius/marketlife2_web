using Model.ShopObjects;

namespace Events
{
    public struct CashDeskHireStaffButtonClickedEvent
    {
        public readonly CashDeskModel CashDeskModel;

        public CashDeskHireStaffButtonClickedEvent(CashDeskModel cashDeskModel)
        {
            CashDeskModel = cashDeskModel;
        }
    }
}