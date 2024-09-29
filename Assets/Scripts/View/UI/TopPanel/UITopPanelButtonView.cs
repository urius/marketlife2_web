using UnityEngine;
using View.UI.Common;

namespace View.UI.TopPanel
{
    public class UITopPanelButtonView : UISimpleButtonView
    {
        [SerializeField] private Transform _newNotificationTransform;

        public void SetNewNotificationVisibility(bool isVisible)
        {
            _newNotificationTransform.gameObject.SetActive(isVisible);
        }
    }
}