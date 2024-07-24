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
            if (index < _truckPointsSettings.Length)
            {
                setting = _truckPointsSettings[index];
                return true;
            }

            setting = default;
            return false;
        }
    }
}