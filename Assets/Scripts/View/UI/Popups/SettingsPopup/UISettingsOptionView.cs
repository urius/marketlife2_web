using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Popups.SettingsPopup
{
    public class UISettingsOptionView : MonoBehaviour
    {
        public event Action<bool> ToggleValueChanged;
        
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Toggle _toggle;

        private void Awake()
        {
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveAllListeners();
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetToggleState(bool isEnabled)
        {
            _toggle.isOn = isEnabled;
        }

        private void OnValueChanged(bool value)
        {
            ToggleValueChanged?.Invoke(value);
        }
    }
}