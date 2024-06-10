using System.Collections.Generic;
using System.Linq;
using Data;
using Infra.Instance;
using View.Game.ShopObjects.Shelf;

namespace Holders
{
    public class ShelfSettingsProvider : IShelfSettingsProvider
    {
        private readonly PrefabsHolderSo _prefabsHolder = Instance.Get<PrefabsHolderSo>();
        private readonly Dictionary<ShopObjectType, ShelfSettingsData[]> _shelfSettings = new();

        public ShelfSettingsProvider()
        {
            FillShelfSettingsDictionary();
        }

        public bool TryGetShelfSetting(ShopObjectType shopObjectType, int upgradeIndex, out ShelfSettingsData settingsData)
        {
            settingsData = default;
            
            if (_shelfSettings.TryGetValue(shopObjectType, out var settingsList) 
                && upgradeIndex < settingsList.Length)
            {
                settingsData = settingsList[upgradeIndex];
                return true;
            }

            return false;
        }

        private void FillShelfSettingsDictionary()
        {
            var shelfSettingsTemp = new Dictionary<ShopObjectType, LinkedList<ShelfSettingsData>>();

            foreach (var prefabsHolderItem in _prefabsHolder.Items)
            {
                var shelfView = prefabsHolderItem.Prefab.GetComponent<ShelfView>();

                if (shelfView != null)
                {
                    var prefabKey = prefabsHolderItem.Key;
                    var shelfType = shelfView.ShelfType;
                    var upgradeIndex = shelfView.ShelfUpgradeIndex;

                    shelfSettingsTemp.TryAdd(shelfType, new LinkedList<ShelfSettingsData>());

                    shelfSettingsTemp[shelfType].AddLast(
                        new ShelfSettingsData()
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

        public struct ShelfSettingsData
        {
            public PrefabKey PrefabKey;
            public int UpgradeIndex;
            public int SlotsAmount;
        }
    }

    public interface IShelfSettingsProvider
    {
        public bool TryGetShelfSetting(ShopObjectType shopObjectType, int upgradeIndex,
            out ShelfSettingsProvider.ShelfSettingsData settingsData);
    }
}