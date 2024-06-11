using System;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;
using View.Game.Shared;

namespace View.Camera
{
    public class MainCameraMediator : MediatorBase
    {
        private readonly IMainCameraHolder _mainCameraHolder = Instance.Get<IMainCameraHolder>();
        private readonly IPlayerCharViewSharedDataHolder _playerCharViewSharedDataHolder = Instance.Get<IPlayerCharViewSharedDataHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private UnityEngine.Camera _mainCamera;
        private Vector2 _cameraOffset;
        private float _cameraZPos;
        private Vector2 _playerCharPos;

        protected override async void MediateInternal()
        {
            _mainCamera = _mainCameraHolder.MainCamera;
            _cameraZPos = _mainCamera.transform.position.z;

            CalculateCameraOffset();

            await _playerCharViewSharedDataHolder.PlayerCharViewSetTask;

            _playerCharPos = _playerCharViewSharedDataHolder.PlayerCharPosition;
            PointCameraToWorldPos(_playerCharPos);

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _updatesProvider.FixedUpdateHappened += OnFixedUpdateHappened;
            _eventBus.Subscribe<PlayerCharPositionChangedEvent>(OnPlayerCharPositionChanged);
        }

        private void Unsubscribe()
        {
            _updatesProvider.FixedUpdateHappened -= OnFixedUpdateHappened;
            _eventBus.Unsubscribe<PlayerCharPositionChangedEvent>(OnPlayerCharPositionChanged);
        }

        private void OnPlayerCharPositionChanged(PlayerCharPositionChangedEvent e)
        {
            _playerCharPos = e.Position;
        }

        private void OnFixedUpdateHappened()
        {
            FollowMainChar();
        }

        private void FollowMainChar()
        {
            var cameraPos = GetCameraOnPlanePosition();
            var deltaPos = _playerCharPos - cameraPos;

            if (deltaPos.sqrMagnitude > 0.01f)
            {
                var cameraMoveOffset = deltaPos * 0.1f;
                PointCameraToWorldPos(cameraPos + cameraMoveOffset);
            }
        }

        private void CalculateCameraOffset()
        {
            var plane = new Plane(Vector3.forward, Vector3.zero);

            var transform = _mainCamera.transform;
            var cameraPos = transform.position;
            var ray = new Ray(cameraPos, transform.forward);

            if (plane.Raycast(ray, out var distance))
            {
                var hitPoint = ray.GetPoint(distance);
                _cameraOffset = new Vector2(cameraPos.x - hitPoint.x, cameraPos.y - hitPoint.y);

                PointCameraToWorldPos(Vector2.zero);
            }
            else
            {
                throw new InvalidOperationException(
                    $"{nameof(CalculateCameraOffset)} unable to calculate camera offset");
            }
        }

        private void PointCameraToWorldPos(Vector2 position)
        {
            var cameraPos2 = position + _cameraOffset;
            var cameraPos = new Vector3(cameraPos2.x, cameraPos2.y, _cameraZPos);

            _mainCamera.transform.position = cameraPos;
        }

        private Vector2 GetCameraOnPlanePosition()
        {
            var pos = _mainCamera.transform.position;
            
            return new Vector2(pos.x, pos.y) - _cameraOffset;
        }
    }
}