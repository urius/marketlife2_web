using Data;
using Model.ShopObjects;

namespace Model.People.States.Staff
{
    public class TruckPointStaffPutProductsOnShelfState : BotCharStateBase
    {
        public readonly ShelfModel TargetShelf;

        public TruckPointStaffPutProductsOnShelfState(ShelfModel targetShelf)
        {
            TargetShelf = targetShelf;
        }
        public override ShopCharStateName StateName => ShopCharStateName.TpStaffPutProductsToShelf;
    }
}