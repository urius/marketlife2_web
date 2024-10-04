using Holders;
using Infra.Instance;
using View.Extensions;
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
        
        private UIRootView _view;

        protected override async void MediateInternal()
        {
            _view = TargetTransform.GetComponent<UIRootView>();
            
            _view.UILoadingOverlayImage.gameObject.SetActive(true);
            
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
        }

        private void AnimateLoadingOverlayFadeOut()
        {
            var image = _view.UILoadingOverlayImage;
            
            LeanTween.value(image.color.a, 0f, 0.5f)
                .setOnUpdate(image.SetAlpha)
                .setOnComplete(_view.RemoveLoadingOverlay);
        }
    }
}