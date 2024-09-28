using Data;
using Events;
using Extensions;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Tools.AudioManager;

namespace Commands
{
    public class UpgradeShelfCommand : ICommand<UIShelfUpgradeClickedEvent>
    {
        private readonly IUpgradeCostProvider _upgradeCostProvider = Instance.Get<IUpgradeCostProvider>();
        private readonly IShelfUpgradeSettingsProvider _shelfUpgradeSettingsProvider = Instance.Get<IShelfUpgradeSettingsProvider>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        public void Execute(UIShelfUpgradeClickedEvent e)
        {
            var shelfModel = e.TargetShelfModel;
            var playerModel = _playerModelHolder.PlayerModel;

            if (_shelfUpgradeSettingsProvider.TryGetShelfUpgradeSetting(
                    shelfModel.ShopObjectType, shelfModel.UpgradeIndex + 1, out var shelfSettings))
            {
                var upgradeCost = _upgradeCostProvider.GetShelfUpgradeCost(shelfModel);
                if (upgradeCost > 0 
                    && playerModel.TrySpendMoney(upgradeCost))
                {
                    _audioPlayer.PlaySound(SoundIdKey.CashSound_2);
                    
                    shelfModel.SetSlotsAmount(shelfSettings.SlotsAmount);
                    shelfModel.IncrementUpgradeIndex();
                }
            }
        }
    }
}