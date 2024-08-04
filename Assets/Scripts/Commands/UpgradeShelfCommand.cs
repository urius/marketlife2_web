using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;

namespace Commands
{
    public class UpgradeShelfCommand : ICommand<UIShelfUpgradeClickedEvent>
    {
        private readonly IUpgradeCostProvider _upgradeCostProvider = Instance.Get<IUpgradeCostProvider>();
        private readonly IShelfUpgradeSettingsProvider _shelfUpgradeSettingsProvider = Instance.Get<IShelfUpgradeSettingsProvider>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        
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
                    shelfModel.SetSlotsAmount(shelfSettings.SlotsAmount);
                    shelfModel.IncrementUpgradeIndex();
                }
            }
        }
    }
}