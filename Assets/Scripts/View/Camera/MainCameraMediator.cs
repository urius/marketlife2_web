using System;
using System.Collections.Generic;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model.SpendPoints;
using UnityEngine;
using Utils;
using View.Game.Shared;

namespace View.Camera
{
    public class MainCameraMediator : MediatorBase
    {
        private const int ShowPositionsDelayFrames = 20;
        
        private readonly IMainCameraHolder _mainCameraHolder = Instance.Get<IMainCameraHolder>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IPlayerCharViewSharedDataHolder _playerCharViewSharedDataHolder = Instance.Get<IPlayerCharViewSharedDataHolder>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IPlayerFocusSetter _playerFocusSetter = Instance.Get<IPlayerFocusSetter>();

        private readonly Queue<ShowPositionItemData> _showPositionsQueue = new();
        
        private UnityEngine.Camera _mainCamera;
        private Vector2 _cameraOffset;
        private float _cameraZPos;
        private Vector2 _playerCharPos;
        private Action _fixedUpdateAction;
        private int _showPositionsDelayFramesLeft;

        protected override async void MediateInternal()
        {
            _mainCamera = _mainCameraHolder.MainCamera;
            _cameraZPos = _mainCamera.transform.position.z;

            CalculateCameraOffset();

            await _playerCharViewSharedDataHolder.PlayerCharViewSetTask;

            _playerCharPos = _playerCharViewSharedDataHolder.PlayerCharPosition;
            PointCameraToWorldPos(_playerCharPos);

            _fixedUpdateAction = FollowMainChar;
            _playerFocusSetter.SetPlayerFocusedFlag(true);

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private async void Subscribe()
        {
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
            _eventBus.Subscribe<PlayerCharPositionChangedEvent>(OnPlayerCharPositionChanged);
            _eventBus.Subscribe<ExpandPointUnlockedEvent>(OnExpandPointUnlockedEvent);

            await _playerModelHolder.PlayerModelSetTask;

            _playerModelHolder.PlayerModel.ShopModel.BuildPointAdded += OnBuildPointAdded;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            _eventBus.Unsubscribe<PlayerCharPositionChangedEvent>(OnPlayerCharPositionChanged);
            _eventBus.Unsubscribe<ExpandPointUnlockedEvent>(OnExpandPointUnlockedEvent);
            
            _playerModelHolder.PlayerModel.ShopModel.BuildPointAdded -= OnBuildPointAdded;
        }

        private void OnExpandPointUnlockedEvent(ExpandPointUnlockedEvent e)
        {
            var worldPosition = _gridCalculator.GetCellCenterWorld(e.BuildPointModel.CellCoords);
            
            AddShowPosition(worldPosition, DispatchExpandPointShown);

            SetDelayState();
        }

        private void DispatchExpandPointShown(Vector2 worldPosition)
        {
            var cellPosition = _gridCalculator.WorldToCell(worldPosition);
            
            _eventBus.Dispatch(new ExpandPointShownEvent(cellPosition));
        }

        private void OnBuildPointAdded(BuildPointModel buildPointModel)
        {
            var worldPosition = _gridCalculator.GetCellCenterWorld(buildPointModel.CellCoords);
            
            AddShowPosition(worldPosition);

            SetDelayState();
        }

        private void SetDelayState()
        {
            ResetDelay();
            _fixedUpdateAction = ShowRequestedPositionsDelay;
        }

        private void AddShowPosition(Vector3 worldPosition, Action<Vector2> shownAction = null)
        {
            var showPositionData = new ShowPositionItemData()
            {
                WorldPosition = worldPosition,
                PositionShownAction = shownAction,
            };
            _showPositionsQueue.Enqueue(showPositionData);
        }

        private void ResetDelay()
        {
            _showPositionsDelayFramesLeft = ShowPositionsDelayFrames;
        }

        private void OnPlayerCharPositionChanged(PlayerCharPositionChangedEvent e)
        {
            _playerCharPos = e.Position;
        }

        private void OnGameplayFixedUpdate()
        {
            _fixedUpdateAction?.Invoke();
        }

        private void FollowMainChar()
        {
            var followCameraResult = MoveCameraToPosition(_playerCharPos);
            
            if (followCameraResult == false)
            {
                _playerFocusSetter.SetPlayerFocusedFlag(true);
            }
        }

        private void ShowRequestedPositionsDelay()
        {
            if (_showPositionsDelayFramesLeft > 0)
            {
                _showPositionsDelayFramesLeft--;
                
                if (_showPositionsQueue.Count > 0)
                {
                    _playerFocusSetter.SetPlayerFocusedFlag(false);
                }
            }
            else
            {
                _fixedUpdateAction = _showPositionsQueue.Count > 0 ? ShowRequestedPositions : FollowMainChar;
            }
        }

        private void ShowRequestedPositions()
        {
            var showPositionData = _showPositionsQueue.Peek();
            
            var wasCameraMoved = MoveCameraToPosition(showPositionData.WorldPosition);
            if (wasCameraMoved == false)
            {
                _showPositionsQueue.Dequeue();

                showPositionData.PositionShownAction?.Invoke(showPositionData.WorldPosition);
                
                SetDelayState();
            }
        }

        private bool MoveCameraToPosition(Vector2 targetWorldPosition)
        {
            var cameraPos = GetCameraOnPlanePosition();
            var deltaPos = targetWorldPosition - cameraPos;

            const float cameraDeltaPosThreshold = 0.005f;
            
            if (deltaPos.sqrMagnitude <= cameraDeltaPosThreshold) return false;
            
            var cameraMoveOffset = deltaPos * 0.1f;
            var newCameraPos = cameraPos + cameraMoveOffset;
            PointCameraToWorldPos(newCameraPos);

            return true;
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
        
        private struct ShowPositionItemData
        {
            public Vector2 WorldPosition;
            public Action<Vector2> PositionShownAction;
        }
    }
}