using Model.ShopObjects;

namespace Events
{
    public struct TruckPointHireStaffButtonClickedEvent
    {
        public readonly TruckPointModel TruckPointModel;

        public TruckPointHireStaffButtonClickedEvent(TruckPointModel truckPointModel)
        {
            TruckPointModel = truckPointModel;
        }
    }
}