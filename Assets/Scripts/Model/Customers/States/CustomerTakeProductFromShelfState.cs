using Data;
using Model.ShopObjects;

namespace Model.Customers.States
{
    public class CustomerTakeProductFromShelfState : CustomerStateBase
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

        public override CustomerGlobalStateName StateName => CustomerGlobalStateName.TakingProduct;
    }
}