using UnityEngine;
using View.UI.Common;

namespace View.UI.TopPanel
{
    public class UITopPanelButtonsView : MonoBehaviour
    {
        [SerializeField] private UISimpleButtonView _interiorButton;

        public UISimpleButtonView InteriorButton => _interiorButton;
    }
}