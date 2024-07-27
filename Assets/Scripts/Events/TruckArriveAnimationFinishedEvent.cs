using Model.ShopObjects;

namespace Events
{
    public struct TruckArriveAnimationFinishedEvent
    {
        public readonly TruckPointModel TruckPointModel;

        public TruckArriveAnimationFinishedEvent(TruckPointModel truckPointModel)
        {
            TruckPointModel = truckPointModel;
        }
    }
}