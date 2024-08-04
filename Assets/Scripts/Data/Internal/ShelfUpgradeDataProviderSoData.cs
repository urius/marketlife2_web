using System;

namespace Data.Internal
{
    [Serializable]
    public struct ShelfUpgradeDataProviderSoData
    {
        public ShopObjectType ShelfType;
        public PrefabKey[] UpgradePrefabKeys;
    }
}