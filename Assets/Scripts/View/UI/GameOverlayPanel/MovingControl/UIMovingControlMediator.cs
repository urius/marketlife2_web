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
        private const int BlindZoneRadiusSqr = 25;
        
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IMainCameraHolder _mainCameraHolder = Instance.Get<IMainCameraHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();

        private UIMovingControlView _movingControlView;
        private RectTransform _rectTransform;
        private Vector2 _viewZeroPoint;
        private Vector3 _lastMousePosition;
        private Vector2 _directionVector;

        protected override void MediateInternal()
        {
            _rectTransform = TargetTransform as RectTransform;
            
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
            
            _updatesProvider.FixedUpdateHappened -= OnFixedUpdateHappened;
        }

        private void OnGameLayerPointerDownEvent(GameLayerPointerDownEvent e)
        {
            _movingControlView ??= CreateView();

            _viewZeroPoint = GetLocalMousePoint();;
            _movingControlView.SetAnchoredPosition(_viewZeroPoint);
            _movingControlView.ResetAndActivate();

            _updatesProvider.FixedUpdateHappened -= OnFixedUpdateHappened;
            _updatesProvider.FixedUpdateHappened += OnFixedUpdateHappened;
        }

        private void OnFixedUpdateHappened()
        {
            if (Input.mousePosition == _lastMousePosition) return;
            
            _lastMousePosition = Input.mousePosition;
            var point = GetLocalMousePoint();

            var innerPartLocalPos = point - _viewZeroPoint;
            _movingControlView.SetInnerPartPosition(innerPartLocalPos);

            _eventBus.Dispatch(innerPartLocalPos.sqrMagnitude > BlindZoneRadiusSqr
                ? new MovingVectorChangedEvent(innerPartLocalPos.normalized)
                : new MovingVectorChangedEvent(Vector2.zero));
        }

        private Vector2 GetLocalMousePoint()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform, Input.mousePosition, _mainCameraHolder.MainCamera, out var point);
            
            return point;
        }

        private void OnGameLayerPointerUpEvent(GameLayerPointerUpEvent obj)
        {
            _movingControlView ??= CreateView();
            _movingControlView.Deactivate();
            
            _updatesProvider.FixedUpdateHappened -= OnFixedUpdateHappened;
            
            _eventBus.Dispatch(new MovingVectorChangedEvent(Vector2.zero));
        }

        private UIMovingControlView CreateView()
        {
            var viewGo = InstantiatePrefab(PrefabKey.UIInputMovingControl, TargetTransform);
            
            return viewGo.GetComponent<UIMovingControlView>();
        }
    }
}