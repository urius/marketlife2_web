using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Popups.DressesPopup
{
    public class UIDressesPopupItemContentBottomDressView : MonoBehaviour
    {
        [SerializeField] private Image _leg1DressImage; 
        [SerializeField] private Image _leg2DressImage;
        
        public Image Leg1DressImage => _leg1DressImage;
        public Image Leg2DressImage => _leg2DressImage;
    }
}