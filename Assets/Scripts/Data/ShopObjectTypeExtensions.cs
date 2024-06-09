namespace Data
{
    public static class ShopObjectTypeExtensions
    {
        public static bool IsShelf(this ShopObjectType shopObjectType)
        {
            switch (shopObjectType)
            {
                case ShopObjectType.Shelf1:
                case ShopObjectType.Shelf2:
                case ShopObjectType.Shelf3:
                case ShopObjectType.Shelf4:
                    return true;
                default:
                    return false;
            }
        }
    }
}