using Holders;
using Infra.Instance;
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
        }

        protected override void UnmediateInternal()
        {
        }
    }
}