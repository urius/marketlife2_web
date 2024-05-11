using System;

namespace Model
{
    public class PlayerModel
    {
        public event Action<int> MoneyChanged;
        
        public readonly ShopModel ShopModel;
        public readonly PlayerCharModel PlayerCharModel;

        public PlayerModel(ShopModel shopModel, int money, PlayerCharModel playerCharModel)
        {
            ShopModel = shopModel;
            Money = money;
            PlayerCharModel = playerCharModel;
        }
        
        public int Money { get; private set; }

        public void ChangeMoney(int deltaMoney)
        {
            Money += deltaMoney;
            
            MoneyChanged?.Invoke(deltaMoney);
        }
    }
}