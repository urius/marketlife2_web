using Data;
using Holders;
using Infra.Instance;
using View.UI.BottomPanel;
using View.UI.Common;
using View.UI.GameOverlayPanel;
using View.UI.LeftPanel;
using View.UI.Popups;
using View.UI.TopPanel;
using View.UI.Tutorial;

namespace View.UI
{
    public class UIRootMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private UIRootView _view;

        protected override async void MediateInternal()
        {
            _view = TargetTransform.GetComponent<UIRootView>();
            _sharedViewsDataHolder.RegisterSettingsCanvasView(_view.UISettingsCanvasView);
            
            ShowLoadingOverlay();

            MediateChild<UIGameOverlayPanelMediator>(_view.UIGameOverlayPanelTransform);
            
            await _playerModelHolder.PlayerModelSetTask;
            
            MediateChild<UITopPanelMoneyViewMediator>(_view.UITopPanelMoneyView.transform);
            MediateChild<UITopPanelLevelViewMediator>(_view.UITopPanelLevelView.transform);
            MediateChild<UITopPanelButtonsViewMediator>(_view.UITopPanelButtonsView.transform);
            
            MediateChild<UITruckPointPanelMediator>(_view.UIBottomPanelTruckPointPanelView.transform);
            MediateChild<UICashDeskPanelMediator>(_view.UICashDeskPanelView.transform);
            MediateChild<UIShelfPanelMediator>(_view.UIShelfPanelView.transform);
            
            MediateChild<UIFlyingTextsMediator>(TargetTransform);
            MediateChild<UITutorialMediator>(_view.UITutorialRootCanvasView.transform);
            MediateChild<UIPopupsMediator>(_view.UIPopupsCanvasRootView.transform);
            MediateChild<UILeftPanelMediator>(_view.UILeftPanelView.transform);

            AnimateLoadingOverlayFadeOut();
        }

        protected override void UnmediateInternal()
        {
            _sharedViewsDataHolder.UnregisterSettingsCanvasView();
        }

        private void ShowLoadingOverlay()
        {
            _view.UILoadingOverlayView.SetActive(true);
            
            var waitText = _localizationProvider.GetLocale(Constants.LocalizationKeyWaitLoading);
            _view.UILoadingOverlayView.SetMessageText(waitText);
        }

        private void AnimateLoadingOverlayFadeOut()
        {
            var overlayView = _view.UILoadingOverlayView;
            
            overlayView.SetMessageTextVisibility(isVisible: false);
            
            LeanTween.value(overlayView.Alpha, 0f, 0.5f)
                .setOnUpdate(overlayView.SetAlpha)
                .setOnComplete(_view.RemoveLoadingOverlay);
        }
    }
}