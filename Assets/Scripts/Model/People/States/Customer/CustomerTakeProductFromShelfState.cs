using Data;
using Model.ShopObjects;

namespace Model.People.States.Customer
{
    public class CustomerTakeProductFromShelfState : BotCharStateBase
    {
        public readonly ShelfModel TargetShelfModel;
        public readonly int SlotIndex;
        public readonly ProductType ProductType;
        public readonly int BasketSlotIndex;

        public CustomerTakeProductFromShelfState(ShelfModel targetShelfModel, int slotIndex, ProductType productType,
            int basketSlotIndex)
        {
            TargetShelfModel = targetShelfModel;
            SlotIndex = slotIndex;
            ProductType = productType;
            BasketSlotIndex = basketSlotIndex;
        }

        public override ShopCharStateName StateName => ShopCharStateName.CustomerTakingProduct;
    }
}