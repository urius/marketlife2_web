using Model.ShopObjects;

namespace Events
{
    public struct CashDeskHireStaffButtonClickedEvent
    {
        public readonly CashDeskModel CashDeskModel;
        public readonly int HireStaffCost;

        public CashDeskHireStaffButtonClickedEvent(CashDeskModel cashDeskModel, int hireStaffCost)
        {
            CashDeskModel = cashDeskModel;
            HireStaffCost = hireStaffCost;
        }
    }
}