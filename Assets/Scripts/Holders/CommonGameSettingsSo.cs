using Data.Internal;
using UnityEngine;

namespace Holders
{
    [CreateAssetMenu(fileName = "CommonGameSettingsSo", menuName = "ScriptableObjects/CommonGameSettingsSo")]
    public class CommonGameSettingsSo : ScriptableObject, ICommonGameSettings
    {
        [SerializeField] private TextAsset _levelsDataJson;

        private CommonGameSettingsData _commonGameSettingsData;
        
        private void OnEnable()
        {
            _commonGameSettingsData = JsonUtility.FromJson<CommonGameSettingsData>(_levelsDataJson.text);
        }

        public int GetLevelIndexByMoneyAmount(int moneyAmount)
        {
            var result = 0;
            
            for (var levelIndex = 0; levelIndex < _commonGameSettingsData.levels.Length; levelIndex++)
            {
                var levelMoney = _commonGameSettingsData.levels[levelIndex];
                if (levelMoney <= moneyAmount)
                {
                    result = levelIndex;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public int GetLevelTargetMoney(int targetLevelIndex)
        {
            if (targetLevelIndex < _commonGameSettingsData.levels.Length)
            {
                return _commonGameSettingsData.levels[targetLevelIndex];
            }

            return -1;
        }
    }

    public interface ICommonGameSettings
    {
        public int GetLevelIndexByMoneyAmount(int moneyAmount);
        public int GetLevelTargetMoney(int targetLevelIndex);
    }
}