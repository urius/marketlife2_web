using UnityEngine;
using View.UI.BottomPanel;
using View.UI.TopPanel;
using View.UI.Tutorial;

namespace View.UI
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private Transform _gamePanelTransform;
        [SerializeField] private UITopPanelMoneyView _moneyView;
        [SerializeField] private UITopPanelLevelView _levelView;
        [SerializeField] private UITruckPointPanelView _truckPointPanelView;
        [SerializeField] private UIShelfPanelView _shelfPanelView;
        [SerializeField] private UICashDeskPanelView _cashDeskPanelView;
        [SerializeField] private UITutorialRootCanvasView _tutorialRootCanvasView;
        
        public Transform UIGameOverlayPanelTransform => _gamePanelTransform;
        public UITopPanelMoneyView UITopPanelMoneyView => _moneyView;
        public UITopPanelLevelView UITopPanelLevelView => _levelView;
        public UITruckPointPanelView UIBottomPanelTruckPointPanelView => _truckPointPanelView;
        public UIShelfPanelView UIShelfPanelView => _shelfPanelView;
        public UICashDeskPanelView UICashDeskPanelView => _cashDeskPanelView;
        public UITutorialRootCanvasView TutorialRootCanvasView => _tutorialRootCanvasView;
    }
}