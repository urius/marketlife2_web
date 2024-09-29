using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Popups.TabbedContentPopup
{
    public interface IUITabbedContentPopupTabButton
    {
        public RectTransform RectTransform { get; }
        public Button Button { get; }
        public void SetText(string text);
        public void SetNewNotificationVisibility(bool isVisible);
    }
}