using Holders;
using Infra.Instance;
using View.UI.BottomPanel;
using View.UI.Common;
using View.UI.GameOverlayPanel;
using View.UI.TopPanel;

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
            MediateChild<UITruckPointPanelMediator>(view.UIBottomPanelTruckPointPanelView.transform);
            MediateChild<UIFlyingTextsMediator>(TargetTransform);
        }

        protected override void UnmediateInternal()
        {
        }
    }
}