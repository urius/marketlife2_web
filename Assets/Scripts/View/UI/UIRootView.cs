using UnityEngine;
using View.UI.TopPanel;

namespace View.UI
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private Transform _gamePanelTransform;
        [SerializeField] private UITopPanelMoneyView _moneyView;
        [SerializeField] private UITopPanelLevelView _levelView;
        
        public Transform UIGameOverlayPanelTransform => _gamePanelTransform;
        public UITopPanelMoneyView UITopPanelMoneyView => _moneyView;
        public UITopPanelLevelView UITopPanelLevelView => _levelView;
    }
}