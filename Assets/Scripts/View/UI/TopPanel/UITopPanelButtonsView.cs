using UnityEngine;

namespace View.UI.TopPanel
{
    public class UITopPanelButtonsView : MonoBehaviour
    {
        [SerializeField] private UITopPanelButtonView _interiorButton;
        [SerializeField] private UITopPanelButtonView _dressesButton;

        public UITopPanelButtonView InteriorButton => _interiorButton;
        public UITopPanelButtonView DressesButton => _dressesButton;
    }
}