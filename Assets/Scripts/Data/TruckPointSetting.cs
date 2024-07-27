using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct TruckPointSetting
    {
        public int DeliverTimeSecondsDefault;
        public int InitialDeliverTimeUpgradeValue;
        [Space]
        public ProductType[] Products;
    }
}