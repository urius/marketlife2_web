using Data;
using Model.ShopObjects;

namespace Model.People.States.Staff
{
    public class StaffTakeProductFromTruckPointState : BotCharStateBase
    {
        public readonly TruckPointModel TruckPointModel;
        public readonly int ProductBoxIndexToTake;

        public StaffTakeProductFromTruckPointState(TruckPointModel truckPointModel, int productBoxIndexToTake)
        {
            TruckPointModel = truckPointModel;
            ProductBoxIndexToTake = productBoxIndexToTake;
        }

        public override ShopCharStateName StateName => ShopCharStateName.TpStaffTakingProductsFromTruckPoint;
    }
}