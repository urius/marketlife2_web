using UnityEngine;
using View.UI.BottomPanel;
using View.UI.TopPanel;

namespace View.UI
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private Transform _gamePanelTransform;
        [SerializeField] private UITopPanelMoneyView _moneyView;
        [SerializeField] private UITopPanelLevelView _levelView;
        [SerializeField] private UITruckPointPanelView _truckPointPanelView;
        
        public Transform UIGameOverlayPanelTransform => _gamePanelTransform;
        public UITopPanelMoneyView UITopPanelMoneyView => _moneyView;
        public UITopPanelLevelView UITopPanelLevelView => _levelView;
        public UITruckPointPanelView UIBottomPanelTruckPointPanelView => _truckPointPanelView;
    }
}