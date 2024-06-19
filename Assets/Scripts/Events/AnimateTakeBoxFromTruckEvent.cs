using Model.ShopObjects;

namespace Events
{
    public struct AnimateTakeBoxFromTruckEvent
    {
        public readonly TruckPointModel TruckPointModel;
        public readonly int ProductBoxIndexToTake;

        public AnimateTakeBoxFromTruckEvent(TruckPointModel truckPointModel, int productBoxIndexToTake)
        {
            TruckPointModel = truckPointModel;
            ProductBoxIndexToTake = productBoxIndexToTake;
        }
    }
}