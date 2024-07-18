using Data.Internal;
using UnityEngine;

namespace Holders
{
    [CreateAssetMenu(fileName = "LocalizationsHolderSo", menuName = "ScriptableObjects/LocalizationsHolderSo")]
    public class LocalizationsHolderSo : ScriptableObject, ILocalizationProvider
    {
        [SerializeField] private TextAsset _localizationsJson;

        private string _localeLang = "en";
        private LocalizationsData _localizationItems;

        private void OnEnable()
        {
            
            _localizationItems = JsonUtility.FromJson<LocalizationsData>(_localizationsJson.text);
        }

        public string GetLocale(string key)
        {
            foreach (var localizationItem in _localizationItems.localizations)
            {
                if (localizationItem.key == key)
                {
                    return GetLocalizationFromItem(localizationItem);
                }
            }

            return key;
        }

        public void SetLocaleLang(string lang)
        {
            _localeLang = lang.ToLower();
        }

        private string GetLocalizationFromItem(LocalizationItemData localizationItem)
        {
            return _localeLang switch
            {
                "ru" => localizationItem.ru,
                _ => localizationItem.en
            };
        }
    }

    public interface ILocalizationProvider
    {
        public string GetLocale(string key);
    }
}