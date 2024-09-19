using Holders;
using Infra.Instance;
using View.UI.BottomPanel;
using View.UI.Common;
using View.UI.GameOverlayPanel;
using View.UI.Popups;
using View.UI.TopPanel;
using View.UI.Tutorial;

namespace View.UI
{
    public class UIRootMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        
        protected override async void MediateInternal()
        {
            var view = TargetTransform.GetComponent<UIRootView>();
            
            MediateChild<UIGameOverlayPanelMediator>(view.UIGameOverlayPanelTransform);
            
            await _playerModelHolder.PlayerModelSetTask;
            
            MediateChild<UITopPanelMoneyViewMediator>(view.UITopPanelMoneyView.transform);
            MediateChild<UITopPanelLevelViewMediator>(view.UITopPanelLevelView.transform);
            MediateChild<UITopPanelButtonsViewMediator>(view.UITopPanelButtonsView.transform);
            
            MediateChild<UITruckPointPanelMediator>(view.UIBottomPanelTruckPointPanelView.transform);
            MediateChild<UICashDeskPanelMediator>(view.UICashDeskPanelView.transform);
            MediateChild<UIShelfPanelMediator>(view.UIShelfPanelView.transform);
            
            MediateChild<UIFlyingTextsMediator>(TargetTransform);
            MediateChild<UITutorialMediator>(view.UITutorialRootCanvasView.transform);
            MediateChild<UIPopupsMediator>(view.UIPopupsCanvasRootView.transform);
        }

        protected override void UnmediateInternal()
        {
        }
    }
}