using Events;
using Infra.EventBus;
using Infra.Instance;

namespace View.UI.TopPanel
{
    public class UITopPanelButtonsViewMediator : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private UITopPanelButtonsView _buttonsView;

        protected override void MediateInternal()
        {
            _buttonsView = TargetTransform.GetComponent<UITopPanelButtonsView>();
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            _buttonsView = null;
        }

        private void Subscribe()
        {
            _buttonsView.InteriorButton.Clicked += OnInteriorButtonClicked;
        }

        private void Unsubscribe()
        {
            _buttonsView.InteriorButton.Clicked -= OnInteriorButtonClicked;
        }

        private void OnInteriorButtonClicked()
        {
            _eventBus.Dispatch(new UIInteriorButtonClickedEvent());
        }
    }
}