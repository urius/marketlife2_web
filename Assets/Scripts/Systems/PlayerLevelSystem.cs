using Holders;
using Infra.Instance;
using Model;

namespace Systems
{
    public class PlayerLevelSystem : ISystem
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ICommonGameSettings _commonGameSettings = Instance.Get<ICommonGameSettings>();
        
        private PlayerModel _playerModel;

        public void Start()
        {
            _playerModel = _playerModelHolder.PlayerModel;

            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerModel.MoneyChanged += OnMoneyChanged;
        }

        private void Unsubscribe()
        {
            _playerModel.MoneyChanged -= OnMoneyChanged;
        }

        private void OnMoneyChanged(int moneyAmount)
        {
            var levelIndex = _commonGameSettings.GetLevelIndexByMoneyAmount(_playerModel.MoneyAmount);
            
            if (levelIndex > _playerModel.LevelIndex)
            {
                _playerModel.SetLevel(levelIndex + 1);
            }
        }
    }
}