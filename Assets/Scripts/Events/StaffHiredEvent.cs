using Model.ShopObjects;

namespace Events
{
    public struct StaffHiredEvent
    {
        public readonly ShopObjectModelBase ShopObjectModel;

        public StaffHiredEvent(ShopObjectModelBase shopObjectModel)
        {
            ShopObjectModel = shopObjectModel;
        }
    }
}