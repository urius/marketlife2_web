using UnityEngine;

namespace View.UI.TopPanel
{
    public class UITopPanelButtonsView : MonoBehaviour
    {
        [SerializeField] private UITopPanelButtonView _interiorButton;

        public UITopPanelButtonView InteriorButton => _interiorButton;
    }
}