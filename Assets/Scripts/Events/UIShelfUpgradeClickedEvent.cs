using Model.ShopObjects;

namespace Events
{
    public struct UIShelfUpgradeClickedEvent
    {
        public readonly ShelfModel TargetShelfModel;

        public UIShelfUpgradeClickedEvent(ShelfModel targetShelfModel)
        {
            TargetShelfModel = targetShelfModel;
        }
    }
}