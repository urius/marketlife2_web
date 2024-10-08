using TMPro;
using UnityEngine;
using View.UI.Common;

namespace View.UI.Popups.YesNoPopup
{
    public class UIYesNoPopupView : UIPopupViewBase
    {
        [Space(25)]
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private UITextButtonView _yesButton;
        [SerializeField] private UITextButtonView _noButton;

        public UITextButtonView YesButton => _yesButton;
        public UITextButtonView NoButton => _noButton;
        
        public void SetMessageText(string text)
        {
            _messageText.text = text;
        }
    }
}