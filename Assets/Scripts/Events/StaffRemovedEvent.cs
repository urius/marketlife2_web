using Model.ShopObjects;

namespace Events
{
    public struct StaffRemovedEvent
    {
        public readonly ShopObjectModelBase TargetShopObjectModel;

        public StaffRemovedEvent(ShopObjectModelBase targetShopObjectModel)
        {
            TargetShopObjectModel = targetShopObjectModel;
        }
    }
}