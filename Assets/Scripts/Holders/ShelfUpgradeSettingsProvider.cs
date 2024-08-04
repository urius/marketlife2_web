using System.Collections.Generic;
using System.Linq;
using Data;
using Infra.Instance;
using View.Game.ShopObjects.Shelf;

namespace Holders
{
    public class ShelfUpgradeSettingsProvider : IShelfUpgradeSettingsProvider
    {
        private readonly PrefabsHolderSo _prefabsHolder = Instance.Get<PrefabsHolderSo>();
        private readonly Dictionary<ShopObjectType, ShelfUpgradeSettingsProviderData[]> _shelfSettings = new();

        public ShelfUpgradeSettingsProvider()
        {
            FillShelfSettingsDictionary();
        }

        public bool CanUpgradeTo(ShopObjectType shelfType, int upgradeIndex)
        {
            return TryGetShelfUpgradeSetting(shelfType, upgradeIndex, out _);
        }
        
        public bool TryGetShelfUpgradeSetting(ShopObjectType shopObjectType, int upgradeIndex, out ShelfUpgradeSettingsProviderData upgradeSettingsProviderData)
        {
            upgradeSettingsProviderData = default;
            
            if (_shelfSettings.TryGetValue(shopObjectType, out var settingsList) 
                && upgradeIndex < settingsList.Length)
            {
                upgradeSettingsProviderData = settingsList[upgradeIndex];
                return true;
            }

            return false;
        }

        private void FillShelfSettingsDictionary()
        {
            var shelfSettingsTemp = new Dictionary<ShopObjectType, LinkedList<ShelfUpgradeSettingsProviderData>>();

            foreach (var prefabsHolderItem in _prefabsHolder.Items)
            {
                var shelfView = prefabsHolderItem.Prefab.GetComponent<ShelfView>();

                if (shelfView != null)
                {
                    var prefabKey = prefabsHolderItem.Key;
                    var shelfType = shelfView.ShelfType;
                    var upgradeIndex = shelfView.ShelfUpgradeIndex;

                    shelfSettingsTemp.TryAdd(shelfType, new LinkedList<ShelfUpgradeSettingsProviderData>());

                    shelfSettingsTemp[shelfType].AddLast(
                        new ShelfUpgradeSettingsProviderData()
                        {
                            PrefabKey = prefabKey,
                            UpgradeIndex = upgradeIndex,
                            SlotsAmount = shelfView.SlotsAmount,
                        });
                }
            }

            foreach (var shelfType in shelfSettingsTemp.Keys)
            {
                _shelfSettings[shelfType] = shelfSettingsTemp[shelfType]
                    .OrderBy(d => d.UpgradeIndex)
                    .ToArray();
            }
        }

        public struct ShelfUpgradeSettingsProviderData
        {
            public PrefabKey PrefabKey;
            public int UpgradeIndex;
            public int SlotsAmount;
        }
    }

    public interface IShelfUpgradeSettingsProvider
    {
        public bool TryGetShelfUpgradeSetting(ShopObjectType shopObjectType, int upgradeIndex,
            out ShelfUpgradeSettingsProvider.ShelfUpgradeSettingsProviderData upgradeSettingsProviderData);

        public bool CanUpgradeTo(ShopObjectType shelfType, int upgradeIndex);
    }
}