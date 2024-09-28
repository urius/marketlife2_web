using Data;
using Model.People;
using Model.ShopObjects;

namespace Events
{
    public struct AnimatePutProductOnShelfEvent
    {
        public readonly ShelfModel ShelfModel;
        public readonly ProductBoxModel ProductBoxModel;
        public readonly ProductType ProductToMove;
        public readonly int BoxSlotIndex;
        public readonly int ShelfSlotIndex;
        public readonly bool IsPlayerCharAction;

        public AnimatePutProductOnShelfEvent(
            ShelfModel shelfModel,
            ProductBoxModel productBoxModel,
            ProductType productToMove,
            int boxSlotIndex,
            int shelfSlotIndex,
            bool isPlayerCharAction)
        {
            ShelfModel = shelfModel;
            ProductBoxModel = productBoxModel;
            ProductToMove = productToMove;
            BoxSlotIndex = boxSlotIndex;
            ShelfSlotIndex = shelfSlotIndex;
            IsPlayerCharAction = isPlayerCharAction;
        }
    }
}