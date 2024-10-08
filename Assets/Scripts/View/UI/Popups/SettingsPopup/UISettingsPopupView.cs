using TMPro;
using UnityEngine;
using View.UI.Common;

namespace View.UI.Popups.SettingsPopup
{
    public class UISettingsPopupView : UIPopupViewBase
    {
        [Space(25)]
        [SerializeField] private UISettingsOptionView _soundsOption;
        [SerializeField] private UISettingsOptionView _musicOption;
        [SerializeField] private UITextButtonView _resetPlayerDataButton;
        [SerializeField] private TMP_InputField _idText;

        public UISettingsOptionView SoundsOption => _soundsOption;
        public UISettingsOptionView MusicOption => _musicOption;
        public UITextButtonView ResetPlayerDataButton => _resetPlayerDataButton;

        public void SetIdText(string text)
        {
            _idText.text = text;
        }
    }
}