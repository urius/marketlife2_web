using Holders;
using Infra.Instance;
using Model;
using Model.ShopObjects;

namespace Systems
{
    public class PlayerLevelSystem : ISystem
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ICommonGameSettings _commonGameSettings = Instance.Get<ICommonGameSettings>();
        
        private PlayerModel _playerModel;
        private ShopModel _shopModel;

        public void Start()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            _shopModel = _playerModel.ShopModel;

            UpdateLevelProcessingActiveFlag();

            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerModel.MoneyChanged += OnMoneyChanged;
            _shopModel.ShopObjectAdded += OnShopObjectAdded;
        }

        private void Unsubscribe()
        {
            _playerModel.MoneyChanged -= OnMoneyChanged;
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
        }

        private void OnMoneyChanged(int moneyAmount)
        {
            var levelIndex = _commonGameSettings.GetLevelIndexByMoneyAmount(_playerModel.MoneyAmount);
            
            if (levelIndex > _playerModel.LevelIndex)
            {
                _playerModel.SetLevel(levelIndex + 1);
            }
        }

        private void UpdateLevelProcessingActiveFlag()
        {
            _playerModel.SetIsLevelProcessingActive(_playerModel.ShopModel.ShopObjects.Count > 2);
        }

        private void OnShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            if (!_playerModel.IsLevelProcessingActive)
            {
                UpdateLevelProcessingActiveFlag();
            }
        }
    }
}