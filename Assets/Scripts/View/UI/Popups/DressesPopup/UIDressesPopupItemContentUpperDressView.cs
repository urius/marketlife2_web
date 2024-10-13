using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Popups.DressesPopup
{
    public class UIDressesPopupItemContentUpperDressView : MonoBehaviour
    {
        [SerializeField] private Image _baseDressImage; 
        [SerializeField] private Image _handDressImage;
        
        public Image BaseDressImage => _baseDressImage;
        public Image HandDressImage => _handDressImage;
    }
}