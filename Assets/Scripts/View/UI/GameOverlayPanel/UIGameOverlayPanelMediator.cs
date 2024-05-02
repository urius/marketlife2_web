using Events;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine.EventSystems;
using View.UI.GameOverlayPanel.MovingControl;

namespace View.UI.GameOverlayPanel
{
    public class UIGameOverlayPanelMediator : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private UIGameOverlayPanelView _view;

        protected override void MediateInternal()
        {
            _view = TargetTransform.GetComponent<UIGameOverlayPanelView>();

            MediateChild<UIMovingControlMediator>(TargetTransform);

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _view.OnPointerDownEvent += OnPointerDownHandler;
            _view.OnPointerUpEvent += OnPointerUpHandler;
        }

        private void Unsubscribe()
        {
            _view.OnPointerDownEvent -= OnPointerDownHandler;
            _view.OnPointerUpEvent -= OnPointerUpHandler;
        }

        private void OnPointerDownHandler(PointerEventData eventdata)
        {
            _eventBus.Dispatch(new GameLayerPointerDownEvent());
        }

        private void OnPointerUpHandler(PointerEventData eventdata)
        {
            _eventBus.Dispatch(new GameLayerPointerUpEvent());
        }
    }
}