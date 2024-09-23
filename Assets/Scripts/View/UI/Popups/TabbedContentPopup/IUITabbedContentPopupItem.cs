using UnityEngine;

namespace View.UI.Popups.TabbedContentPopup
{
    public interface IUITabbedContentPopupItem
    {
        public RectTransform RectTransform { get; }
        public Vector2 Size { get; }
    }
}