using Model.ShopObjects;

namespace Events
{
    public struct RequestHireStaffEvent
    {
        public readonly ShopObjectModelBase TargetShopObjectModel;

        public RequestHireStaffEvent(ShopObjectModelBase targetShopObjectModel)
        {
            TargetShopObjectModel = targetShopObjectModel;
        }
    }
}