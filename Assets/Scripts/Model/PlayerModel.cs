using System;

namespace Model
{
    public class PlayerModel
    {
        public event Action<int> MoneyChanged;
        
        public readonly ShopModel ShopModel;
        public readonly PlayerCharModel PlayerCharModel;

        public PlayerModel(ShopModel shopModel, int moneyAmount, PlayerCharModel playerCharModel)
        {
            ShopModel = shopModel;
            MoneyAmount = moneyAmount;
            PlayerCharModel = playerCharModel;
        }
        
        public int MoneyAmount { get; private set; }

        public void ChangeMoney(int deltaMoney)
        {
            MoneyAmount += deltaMoney;
            
            MoneyChanged?.Invoke(deltaMoney);
        }
    }
}