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
        private const int BlindZoneRadius = 20;
        private const int BlindZoneRadiusSqr = BlindZoneRadius * BlindZoneRadius;
        
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IMainCameraHolder _mainCameraHolder = Instance.Get<IMainCameraHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();

        private UIMovingControlView _movingControlView;
        private RectTransform _rectTransform;
        private Vector2 _viewZeroPoint;
        private Vector3 _lastMousePosition;
        private Vector2 _directionVector;
        private bool _keyWPressed = false;
        private bool _keyAPressed = false;
        private bool _keySPressed = false;
        private bool _keyDPressed = false;

        protected override void MediateInternal()
        {
            _rectTransform = TargetTransform as RectTransform;
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<GameLayerPointerDownEvent>(OnGameLayerPointerDownEvent);
            _eventBus.Subscribe<GameLayerPointerUpEvent>(OnGameLayerPointerUpEvent);
            
            _updatesProvider.GameplayFixedUpdate += OnProcessKeyboardGameplayFixedUpdate;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<GameLayerPointerDownEvent>(OnGameLayerPointerDownEvent);
            _eventBus.Unsubscribe<GameLayerPointerUpEvent>(OnGameLayerPointerUpEvent);
            
            _updatesProvider.GameplayFixedUpdate -= OnPointerDownGameplayFixedUpdate;
            _updatesProvider.GameplayFixedUpdate -= OnProcessKeyboardGameplayFixedUpdate;
        }

        private void OnProcessKeyboardGameplayFixedUpdate()
        {
            var keyWPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            var keyAPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            var keySPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            var keyDPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

            if (_keyWPressed != keyWPressed
                || _keyAPressed != keyAPressed
                || _keySPressed != keySPressed
                || _keyDPressed != keyDPressed)
            {
                var moveVector = Vector2.zero;
                if (keyWPressed)
                {
                    moveVector.y = 1;
                }
                else if (keySPressed)
                {
                    moveVector.y = -1;
                }

                if (keyAPressed)
                {
                    moveVector.x = -1;
                }
                else if (keyDPressed)
                {
                    moveVector.x = 1;
                }

                _eventBus.Dispatch(new MovingVectorChangedEvent(moveVector.normalized));
            }

            _keyWPressed = keyWPressed;
            _keyAPressed = keyAPressed;
            _keySPressed = keySPressed;
            _keyDPressed = keyDPressed;
        }

        private void OnGameLayerPointerDownEvent(GameLayerPointerDownEvent e)
        {
            _movingControlView ??= CreateView();

            _viewZeroPoint = GetLocalMousePoint();;
            _movingControlView.SetAnchoredPosition(_viewZeroPoint);
            _movingControlView.ResetAndActivate();

            _updatesProvider.GameplayFixedUpdate -= OnPointerDownGameplayFixedUpdate;
            _updatesProvider.GameplayFixedUpdate += OnPointerDownGameplayFixedUpdate;
        }

        private void OnPointerDownGameplayFixedUpdate()
        {
            if (Input.mousePosition == _lastMousePosition) return;
            
            _lastMousePosition = Input.mousePosition;
            var point = GetLocalMousePoint();

            var innerPartLocalPos = point - _viewZeroPoint;
            _movingControlView.SetInnerPartPosition(innerPartLocalPos);
            
            var moveVector = _movingControlView.GetInnerPartPosition();

            _eventBus.Dispatch(innerPartLocalPos.sqrMagnitude > BlindZoneRadiusSqr
                ? new MovingVectorChangedEvent(moveVector)
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
            
            _updatesProvider.GameplayFixedUpdate -= OnPointerDownGameplayFixedUpdate;
            
            _eventBus.Dispatch(new MovingVectorChangedEvent(Vector2.zero));
        }

        private UIMovingControlView CreateView()
        {
            var viewGo = InstantiatePrefab(PrefabKey.UIInputMovingControl, TargetTransform);
            
            return viewGo.GetComponent<UIMovingControlView>();
        }
    }
}