using Data;
using Other;
using UnityEngine;

namespace Holders
{
    [CreateAssetMenu(fileName = "TruckPointsSettingsProviderSo", menuName = "ScriptableObjects/TruckPointsSettingsProvider")]
    public class TruckPointsSettingsProviderSo : ScriptableObject
    {
        [SerializeField]
        [LabeledArray(nameof(TruckPointSetting.Products))]
        private TruckPointSetting[] _truckPointsSettings;

        public bool TryGetSettingByTruckPointIndex(int index, out TruckPointSetting setting)
        {
            if (HaveSettings(index))
            {
                setting = _truckPointsSettings[index];
                return true;
            }

            setting = default;
            return false;
        }

        public bool HaveSettings(int truckGatesIndex)
        {
            return truckGatesIndex < _truckPointsSettings.Length;
        }

        public int TruckPointSettingsCount => _truckPointsSettings.Length;
    }
}