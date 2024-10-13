using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Popups.DressesPopup
{
    public class UIDressesPopupItemContentHeadView : MonoBehaviour
    {
        [SerializeField] private Image _hairImage; 
        [SerializeField] private Image _glassesImage; 
        
        public Image HairImage => _hairImage;
        public Image GlassesImage => _glassesImage;
    }
}