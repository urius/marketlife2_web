using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct TruckPointSetting
    {
        public int DeliverTimeSecondsDefault;
        [Space]
        public ProductType[] Products;
        [Space]
        public TruckPointUpgradesSetting UpgradesSetting;
    }

    [Serializable]
    public struct TruckPointUpgradesSetting
    {
        public int InitialDeliverTimeUpgradeValue;
        public int MinDeliverTimeSeconds;
    }
}