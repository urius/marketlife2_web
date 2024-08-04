using System.Collections.Generic;
using System.Linq;
using Data.Internal;
using UnityEngine;

namespace Holders
{
    [CreateAssetMenu(fileName = "LocalizationsHolderSo", menuName = "ScriptableObjects/LocalizationsHolderSo")]
    public class LocalizationsHolderSo : ScriptableObject, ILocalizationProvider
    {
        [SerializeField] private TextAsset _localizationsJson;

        private string _localeLang = "en";
        private Dictionary<string, LocalizationItemData> _localizationByKey;

        private void OnEnable()
        {
            var localizationItems = JsonUtility.FromJson<LocalizationsData>(_localizationsJson.text);
            _localizationByKey = localizationItems.localizations.ToDictionary(d => d.key);
        }

        public string GetLocale(string key)
        {
            return _localizationByKey.TryGetValue(key, out var localizationItemData)
                ? GetLocalizationFromItem(localizationItemData)
                : key;
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