using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using Utils;

namespace Systems
{
    public class PlayerLevelSystem : ISystem
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ICommonGameSettings _commonGameSettings = Instance.Get<ICommonGameSettings>();
        private readonly IInteriorDataProvider _interiorDataProvider = Instance.Get<IInteriorDataProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
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
            if (!_playerModel.IsLevelProcessingActive) return;
            
            var levelIndex = _commonGameSettings.GetLevelIndexByMoneyAmount(_playerModel.MoneyAmount);
            
            if (levelIndex > _playerModel.LevelIndex)
            {
                _playerModel.SetLevel(levelIndex + 1);
                UpdateUIFlagsIfNeeded();

                DispatchUnlockedExpandPoints();
            }
        }

        private void UpdateUIFlagsIfNeeded()
        {
            var level = _playerModel.Level;
            var uiFlagsModel = _playerModel.UIFlagsModel;

            if (_interiorDataProvider.GetFloorItemsByLevel(level).Length > 0)
            {
                uiFlagsModel.SetNewFloorsFlag(true);
            }

            if (_interiorDataProvider.GetWallItemsForNextLevel(level).Length > 0)
            {
                uiFlagsModel.SetNewWallsFlag(true);
            }
        }

        private void DispatchUnlockedExpandPoints()
        {
            foreach (var kvp in _shopModel.BuildPoints)
            {
                if (kvp.Value.BuildPointType == BuildPointType.Expand
                    && _playerModel.Level == ExpandShopHelper.GetExpandLevelByExpandPoint(kvp.Value))
                {
                    _eventBus.Dispatch(new ExpandPointUnlockedEvent(kvp.Value));
                }
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