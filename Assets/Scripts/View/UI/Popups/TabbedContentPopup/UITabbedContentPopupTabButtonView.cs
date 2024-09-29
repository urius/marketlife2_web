using UnityEngine;
using View.UI.Common;

namespace View.UI.Popups.TabbedContentPopup
{
    public class UITabbedContentPopupTabButtonView : UITextButtonView, IUITabbedContentPopupTabButton
    {
        [Space(25)]
        [SerializeField] private Transform _newNotificationTransform;

        public Transform NewNotificationTransform => _newNotificationTransform;

        public void SetNewNotificationVisibility(bool isVisible)
        {
            _newNotificationTransform.gameObject.SetActive(isVisible);
        }
    }
}