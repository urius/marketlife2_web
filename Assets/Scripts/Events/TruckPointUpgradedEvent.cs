using Model.ShopObjects;

namespace Events
{
    public struct TruckPointUpgradedEvent
    {
        public readonly TruckPointModel TruckPointModel;

        public TruckPointUpgradedEvent(TruckPointModel truckPointModel)
        {
            TruckPointModel = truckPointModel;
        }
    }
}