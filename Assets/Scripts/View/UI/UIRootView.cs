using UnityEngine;
using UnityEngine.UI;
using View.UI.BottomPanel;
using View.UI.LeftPanel;
using View.UI.Popups;
using View.UI.SettingsCanvas;
using View.UI.TopPanel;
using View.UI.Tutorial;

namespace View.UI
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private Transform _gamePanelTransform;
        [SerializeField] private UITopPanelMoneyView _moneyView;
        [SerializeField] private UITopPanelLevelView _levelView;
        [SerializeField] private UITopPanelButtonsView _topPanelButtonsView;
        [SerializeField] private UITruckPointPanelView _truckPointPanelView;
        [SerializeField] private UIShelfPanelView _shelfPanelView;
        [SerializeField] private UICashDeskPanelView _cashDeskPanelView;
        [SerializeField] private UITutorialRootCanvasView _tutorialRootCanvasView;
        [SerializeField] private UIPopupsCanvasRootView _popupsCanvasRootView;
        [SerializeField] private UILeftPanelView _uiLeftPanelView;
        [SerializeField] private UISettingsCanvasView _settingsCanvasView;
        [SerializeField] private Image _uiLoadingOverlayImage;

        public Transform UIGameOverlayPanelTransform => _gamePanelTransform;
        public UITopPanelMoneyView UITopPanelMoneyView => _moneyView;
        public UITopPanelLevelView UITopPanelLevelView => _levelView;
        public UITopPanelButtonsView UITopPanelButtonsView => _topPanelButtonsView;
        public UITruckPointPanelView UIBottomPanelTruckPointPanelView => _truckPointPanelView;
        public UIShelfPanelView UIShelfPanelView => _shelfPanelView;
        public UICashDeskPanelView UICashDeskPanelView => _cashDeskPanelView;
        public UITutorialRootCanvasView UITutorialRootCanvasView => _tutorialRootCanvasView;
        public UIPopupsCanvasRootView UIPopupsCanvasRootView => _popupsCanvasRootView;
        public UILeftPanelView UILeftPanelView => _uiLeftPanelView;
        public UISettingsCanvasView UISettingsCanvasView => _settingsCanvasView;
        public Image UILoadingOverlayImage => _uiLoadingOverlayImage;

        public void RemoveLoadingOverlay()
        {
            Destroy(_uiLoadingOverlayImage.gameObject);
        }
    }
}