namespace Model
{
    public class PlayerModel
    {
        public int Money;
        public readonly ShopModel ShopModel;

        public PlayerModel(ShopModel shopModel, int money)
        {
            ShopModel = shopModel;
            Money = money;
        }
    }
}