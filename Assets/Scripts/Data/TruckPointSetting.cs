using System;

namespace Data
{
    [Serializable]
    public struct TruckPointSetting
    {
        public ProductType[] Products;
        public int DeliverTimeSeconds;
    }
}