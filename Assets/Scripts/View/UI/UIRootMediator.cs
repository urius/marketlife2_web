using View.UI.GameOverlayPanel;

namespace View.UI
{
    public class UIRootMediator : MediatorBase
    {
        protected override void MediateInternal()
        {
            var view = TargetTransform.GetComponent<UIRootView>();
            
            MediateChild<UIGameOverlayPanelMediator>(view.UIGameOverlayPanelTransform);
        }

        protected override void UnmediateInternal()
        {
        }
    }
}