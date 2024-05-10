namespace Model
{
    public class PlayerModel
    {
        public int Money;
        public readonly ShopModel ShopModel;
        public readonly PlayerCharModel PlayerCharModel;

        public PlayerModel(ShopModel shopModel, int money, PlayerCharModel playerCharModel)
        {
            ShopModel = shopModel;
            Money = money;
            PlayerCharModel = playerCharModel;
        }
    }
}