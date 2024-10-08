using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using Tools;
using UnityEngine;
using Utils;

namespace Systems
{
    public class SaveDataSystem : ISystem
    {
        private const int AutoSaveIntervalSeconds = 30;
        private const int SaveCooldownSeconds = 5;
        
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IPopupViewModelsHolder _popupViewModelsHolder = Instance.Get<IPopupViewModelsHolder>();
        private readonly ISharedFlagsHolder _sharedFlagsHolder = Instance.Get<ISharedFlagsHolder>();
        
        private ShopModel _shopModel;
        private bool _needSaveFlag = false;
        private int _saveCooldownSeconds = 0;
        private PlayerModel _playerModel;
        private PlayerStatsModel _statsModel;
        private int _secondsSinceLastSave;
        private PlayerAudioSettingsModel _audioSettingsModel;

        public void Start()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            _audioSettingsModel = _playerModelHolder.PlayerModel.AudioSettingsModel;
            _shopModel = _playerModelHolder.PlayerModel.ShopModel;
            _statsModel = _playerModel.StatsModel;

            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _shopModel.ShopObjectAdded += OnShopObjectAdded;
            _shopModel.ShopExpanded += OnShopExpanded;
            _shopModel.FloorsTypeUpdated += OnFloorsTypeUpdated;
            _shopModel.WallsTypeUpdated += OnWallsTypeUpdated;
            _playerModel.LevelChanged += OnLevelChanged;
            _playerModel.MoneyEarnModifierAdded += OnMoneyEarnModifierAdded;
            _audioSettingsModel.MusicMutedStateChanged += OnMusicMutedStateChanged;
            _audioSettingsModel.SoundsMutedStateChanged += OnSoundsMutedStateChanged;
            _updatesProvider.RealtimeSecondPassed += OnRealtimeSecondPassed;

            _eventBus.Subscribe<StaffHiredEvent>(OnStaffHiredEvent);
            _eventBus.Subscribe<StaffWorkTimeProlongedEvent>(OnStaffWorkTimeProlongedEvent);
            _eventBus.Subscribe<ShelfUpgradedEvent>(OnShelfUpgradedEvent);
            _eventBus.Subscribe<TruckPointUpgradedEvent>(OnTruckPointUpgradedEvent);
        }

        private void Unsubscribe()
        {
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
            _shopModel.ShopExpanded -= OnShopExpanded;
            _shopModel.FloorsTypeUpdated -= OnFloorsTypeUpdated;
            _shopModel.WallsTypeUpdated -= OnWallsTypeUpdated;
            _playerModel.LevelChanged -= OnLevelChanged;
            _playerModel.MoneyEarnModifierAdded -= OnMoneyEarnModifierAdded;
            _audioSettingsModel.MusicMutedStateChanged -= OnMusicMutedStateChanged;
            _audioSettingsModel.SoundsMutedStateChanged -= OnSoundsMutedStateChanged;
            _updatesProvider.RealtimeSecondPassed -= OnRealtimeSecondPassed;
            
            _eventBus.Unsubscribe<StaffHiredEvent>(OnStaffHiredEvent);
            _eventBus.Unsubscribe<StaffWorkTimeProlongedEvent>(OnStaffWorkTimeProlongedEvent);
            _eventBus.Unsubscribe<ShelfUpgradedEvent>(OnShelfUpgradedEvent);
            _eventBus.Unsubscribe<TruckPointUpgradedEvent>(OnTruckPointUpgradedEvent);
        }

        private void OnRealtimeSecondPassed()
        {
            if (_needSaveFlag == false)
            {
                _secondsSinceLastSave++;
                
                if (_secondsSinceLastSave > AutoSaveIntervalSeconds)
                {
                    ChargeNeedSaveFlag();
                }
            }

            if (_saveCooldownSeconds > 0)
            {
                _saveCooldownSeconds--;
            }
            else
            {
                if (_needSaveFlag
                    && _popupViewModelsHolder.OpenedPopupsCount <= 0
                    && _sharedFlagsHolder.Get(SharedFlagKey.IsGamePaused) == false)
                {
                    Save();
                }
            }
        }

        private void Save()
        {
            _needSaveFlag = false;
            _saveCooldownSeconds = SaveCooldownSeconds;
            _secondsSinceLastSave = 0;

            var playerDataDToSave = _playerModel.ToPlayerDataDto();
            var playerDataToSaveStr = JsonUtility.ToJson(playerDataDToSave);
            
            GamePushWrapper.SavePlayerData(Constants.PlayerDataKey, playerDataToSaveStr, needSync: false);
            GamePushWrapper.SavePlayerData(Constants.PlayerScoreKey, _statsModel.TotalMoneyEarned, needSync: false);

            Debug.Log($"<color=#39FF00>{nameof(SaveDataSystem)} Save!</color>");
        }

        private void OnTruckPointUpgradedEvent(TruckPointUpgradedEvent e)
        {
            ChargeNeedSaveFlag();
        }

        private void OnMoneyEarnModifierAdded(PlayerMoneyEarnModifierModel model)
        {
            ChargeNeedSaveFlag();
        }

        private void OnLevelChanged(int _)
        {
            ChargeNeedSaveFlag();
        }

        private void OnFloorsTypeUpdated(FloorType _)
        {
            ChargeNeedSaveFlag();

        }

        private void OnWallsTypeUpdated(WallType _)
        {
            ChargeNeedSaveFlag();

        }

        private void OnStaffHiredEvent(StaffHiredEvent e)
        {
            ChargeNeedSaveFlag();
        }

        private void OnStaffWorkTimeProlongedEvent(StaffWorkTimeProlongedEvent e)
        {
            ChargeNeedSaveFlag();
        }

        private void OnShelfUpgradedEvent(ShelfUpgradedEvent e)
        {
            ChargeNeedSaveFlag();
        }

        private void OnShopExpanded(Vector2Int _)
        {
            ChargeNeedSaveFlag();
        }

        private void OnShopObjectAdded(ShopObjectModelBase model)
        {
            ChargeNeedSaveFlag();
        }

        private void OnSoundsMutedStateChanged(bool _)
        {
            ChargeNeedSaveFlag();
        }

        private void OnMusicMutedStateChanged(bool _)
        {
            ChargeNeedSaveFlag();
        }

        private void ChargeNeedSaveFlag()
        {
            _needSaveFlag = true;
        }
    }
}