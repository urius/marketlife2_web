using Data;
using Model.ShopObjects;

namespace Events
{
    public struct AnimatePutProductOnShelfEvent
    {
        public readonly ShelfModel ShelfModel;
        public readonly ProductType ProductToMove;
        public readonly int BoxSlotIndex;
        public readonly int ShelfSlotIndex;

        public AnimatePutProductOnShelfEvent(ShelfModel shelfModel, ProductType productToMove, int boxSlotIndex,
            int shelfSlotIndex)
        {
            ShelfModel = shelfModel;
            ProductToMove = productToMove;
            BoxSlotIndex = boxSlotIndex;
            ShelfSlotIndex = shelfSlotIndex;
        }
    }
}