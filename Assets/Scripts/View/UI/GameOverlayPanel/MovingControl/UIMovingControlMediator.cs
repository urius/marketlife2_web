using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;

namespace View.UI.GameOverlayPanel.MovingControl
{
    public class UIMovingControlMediator : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IMainCameraHolder _mainCameraHolder = Instance.Get<IMainCameraHolder>();

        private UIMovingControlView _movingControlView;
        private RectTransform _rectTransform;

        protected override void MediateInternal()
        {
            _rectTransform = Transform as RectTransform;
            
            SubscribeToEvents();
        }

        protected override void UnmediateInternal()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<GameLayerPointerDownEvent>(OnGameLayerPointerDownEvent);
            _eventBus.Subscribe<GameLayerPointerUpEvent>(OnGameLayerPointerUpEvent);
        }

        private void UnsubscribeFromEvents()
        {
            _eventBus.Unsubscribe<GameLayerPointerDownEvent>(OnGameLayerPointerDownEvent);
            _eventBus.Unsubscribe<GameLayerPointerUpEvent>(OnGameLayerPointerUpEvent);
        }

        private void OnGameLayerPointerDownEvent(GameLayerPointerDownEvent e)
        {
            _movingControlView ??= CreateView();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform, Input.mousePosition, _mainCameraHolder.MainCamera, out var point);

            _movingControlView.SetAnchoredPosition(point);
            _movingControlView.ResetAndActivate();
        }

        private void OnGameLayerPointerUpEvent(GameLayerPointerUpEvent obj)
        {
            _movingControlView ??= CreateView();
            _movingControlView.Deactivate();
        }

        private UIMovingControlView CreateView()
        {
            var viewGo = InstantiatePrefab(PrefabKey.UIInputMovingControl, Transform);
            return viewGo.GetComponent<UIMovingControlView>();
        }
    }
}