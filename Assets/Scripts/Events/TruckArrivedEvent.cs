using Model.ShopObjects;

namespace Events
{
    public struct TruckArrivedEvent
    {
        public readonly TruckPointModel TruckPointModel;

        public TruckArrivedEvent(TruckPointModel truckPointModel)
        {
            TruckPointModel = truckPointModel;
        }
    }
}