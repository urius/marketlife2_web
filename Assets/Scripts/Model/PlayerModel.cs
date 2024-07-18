using System;

namespace Model
{
    public class PlayerModel
    {
        public event Action<int> MoneyChanged;
        public event Action<int> LevelChanged;
        
        public readonly ShopModel ShopModel;
        public readonly PlayerCharModel PlayerCharModel;

        public PlayerModel(ShopModel shopModel, int moneyAmount, int level, PlayerCharModel playerCharModel)
        {
            ShopModel = shopModel;
            MoneyAmount = moneyAmount;
            Level = level <= 0 ? 1 : level;
            PlayerCharModel = playerCharModel;
        }
        
        public int MoneyAmount { get; private set; }
        public int Level { get; private set; }
        public int LevelIndex => Level - 1;

        public void ChangeMoney(int deltaMoney)
        {
            MoneyAmount += deltaMoney;
            
            MoneyChanged?.Invoke(MoneyAmount);
        }
        
        public void SetLevel(int level)
        {
            Level = level;

            LevelChanged?.Invoke(Level);
        }
    }
}