using Model.ShopObjects;

namespace Events
{
    public struct ShopObjectCellsRegisteredEvent
    {
        public readonly ShopObjectModelBase ShopObjectModel;

        public ShopObjectCellsRegisteredEvent(ShopObjectModelBase shopObjectModel)
        {
            ShopObjectModel = shopObjectModel;
        }
    }
}