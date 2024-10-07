using Model.ShopObjects;

namespace Events
{
    public struct ShelfUpgradedEvent
    {
        public readonly ShelfModel ShelfModel;

        public ShelfUpgradedEvent(ShelfModel shelfModel)
        {
            ShelfModel = shelfModel;
        }
    }
}