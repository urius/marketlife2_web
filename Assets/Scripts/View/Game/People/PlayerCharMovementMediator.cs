using System;
using System.Collections.Generic;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using UnityEngine;
using Utils;
using View.Game.Shared;

namespace View.Game.People
{
    public class PlayerCharMovementMediator : MediatorBase
    {
        private static readonly Vector2Int CellPosRight = new(1, 0);
        private static readonly Vector2Int CellPosLeft = new(-1, 0);
        private static readonly Vector2Int CellPosDown = new(0, 1);
        private static readonly Vector2Int CellPosUp = new(0, -1);
        
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IPlayerFocusProvider _playerFocusProvider = Instance.Get<IPlayerFocusProvider>();

        private Dictionary<Vector2Int, (Vector3 worldDirection, Vector3 worldDirectionToProject)> _detectorWorldDirections;

        private ManView _playerCharView;
        private Vector2 _moveDirection;
        private Vector2Int _charCellCoords;
        private (Vector2Int direction, Vector3[] offsets)[] _detectorOffsets;
        private Vector3 _playerWorldPosition;
        private Vector2 _prevMoveDirection;
        private PlayerCharModel _playerCharModel;
        private ShopModel _shopModel;

        private bool ShouldPlayerNotMove => _moveDirection == Vector2.zero 
                                            || _playerFocusProvider.IsPlayerFocused == false;

        protected override void MediateInternal()
        {
            _playerCharModel = _playerModelHolder.PlayerModel.PlayerCharModel;
            _shopModel = _playerModelHolder.PlayerModel.ShopModel;
            
            FillDetectorOffsets();
            
            _playerCharView = TargetTransform.GetComponent<ManView>();
            _charCellCoords = _playerCharModel.CellPosition;
            _playerWorldPosition = _playerCharView.transform.position =
                _gridCalculator.GetCellCenterWorld(_charCellCoords);

            UpdateSorting();
            
            Subscribe();
        }

        private void FillDetectorOffsets()
        {
            var cellPrimaryOffset = _gridCalculator.CellSize.x * 0.35f;
            const float innerOffsetMult = 0.2f;
            var xOffsetInner = _gridCalculator.WorldCellXDirection * innerOffsetMult;
            var yOffsetInner = _gridCalculator.WorldCellYDirection * innerOffsetMult;

            _detectorOffsets = new[]
            {
                (CellPosRight, new[]
                {
                    _gridCalculator.WorldCellXDirection * cellPrimaryOffset + yOffsetInner,
                    _gridCalculator.WorldCellXDirection * cellPrimaryOffset - yOffsetInner,
                }),
                (CellPosLeft, new[]
                {
                    -_gridCalculator.WorldCellXDirection * cellPrimaryOffset + yOffsetInner,
                    -_gridCalculator.WorldCellXDirection * cellPrimaryOffset - yOffsetInner,
                }),
                (CellPosDown, new[]
                {
                    _gridCalculator.WorldCellYDirection * cellPrimaryOffset + xOffsetInner,
                    _gridCalculator.WorldCellYDirection * cellPrimaryOffset - xOffsetInner,
                }),
                (CellPosUp, new[]
                {
                    -_gridCalculator.WorldCellYDirection * cellPrimaryOffset + xOffsetInner,
                    -_gridCalculator.WorldCellYDirection * cellPrimaryOffset - xOffsetInner,
                }),
            };

            _detectorWorldDirections =
                new Dictionary<Vector2Int, (Vector3 worldDirection, Vector3 worldDirectionToProject)>
                {
                    { CellPosRight, (_gridCalculator.WorldCellXDirection, _gridCalculator.WorldCellYDirection) },
                    { CellPosLeft, (-_gridCalculator.WorldCellXDirection, _gridCalculator.WorldCellYDirection) },
                    { CellPosDown, (_gridCalculator.WorldCellYDirection, _gridCalculator.WorldCellXDirection) },
                    { CellPosUp, (-_gridCalculator.WorldCellYDirection, _gridCalculator.WorldCellXDirection) },
                };
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<MovingVectorChangedEvent>(OnMovingVectorChangedEvent);
            _eventBus.Subscribe<ShopObjectCellsRegisteredEvent>(OnShopObjectCellsRegisteredEvent);
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
            _playerCharModel.ProductsBoxAdded += OnProductsBoxAdded;
            _playerCharModel.ProductRemoved += OnProductRemoved;
            _playerFocusProvider.PlayerFocusChanged += OnOnPlayerFocusChanged;

            DebugDrawGizmosDispatcher.DrawGizmosHappened += OnDrawGizmosHappened;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<MovingVectorChangedEvent>(OnMovingVectorChangedEvent);
            _eventBus.Unsubscribe<ShopObjectCellsRegisteredEvent>(OnShopObjectCellsRegisteredEvent);
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            _playerCharModel.ProductsBoxAdded -= OnProductsBoxAdded;
            _playerCharModel.ProductRemoved -= OnProductRemoved;
            _playerFocusProvider.PlayerFocusChanged -= OnOnPlayerFocusChanged;
            
            DebugDrawGizmosDispatcher.DrawGizmosHappened -= OnDrawGizmosHappened;
        }

        private void OnOnPlayerFocusChanged(bool isFocused)
        {
            UpdateAnimation();
        }

        private void OnShopObjectCellsRegisteredEvent(ShopObjectCellsRegisteredEvent e)
        {
            var playerIsStayingOnOwnedCell = Array.IndexOf(e.OwnedCells, _playerCharModel.CellPosition) > -1;
            if (playerIsStayingOnOwnedCell)
            {
                foreach (var nearCellOffset in Constants.NearCells8)
                {
                    var nearCell = _playerCharModel.CellPosition + nearCellOffset;
                    if (_ownedCellsDataHolder.IsWalkableForPlayerChar(nearCell))
                    {
                        var worldPosition = _gridCalculator.GetCellCenterWorld(nearCell);
                        SetPlayerCharWorldPosition(worldPosition);
                        
                        return;
                    }
                }
            }
        }

        private void OnProductRemoved(int slotIndex)
        {
            if (_playerCharModel.HasProducts == false)
            {
                UpdateAnimation();
            }
        }

        private void OnGameplayFixedUpdate()
        {
            ProcessMove();
            CheckCellCoords();

            if (_prevMoveDirection != _moveDirection)
            {
                UpdateAnimation();
            }

            _prevMoveDirection = _moveDirection;
        }

        private void OnProductsBoxAdded()
        {
            UpdateAnimation();
        }

        private void CheckCellCoords()
        {
            if (ShouldPlayerNotMove) return;
            
            var newCellCoords = _gridCalculator.WorldToCell(_playerWorldPosition);
            
            if (newCellCoords != _charCellCoords)
            {
                _charCellCoords = newCellCoords;
                
                UpdateSorting();
                
                _eventBus.Dispatch(new RequestPlayerCellChangeEvent(_charCellCoords));
            }
        }

        private void UpdateSorting()
        {
            DynamicViewSortingLogic.UpdateSorting(_playerCharView, _ownedCellsDataHolder, _charCellCoords);
        }

        private void ProcessMove()
        {
            if (ShouldPlayerNotMove) return;
            
            const float speed = 0.06f;

            var clampedMoveDirection = ClampMoveDirection(_moveDirection);

            var delta = clampedMoveDirection * speed;
            _playerWorldPosition += (Vector3)delta;

            SetPlayerCharWorldPosition(_playerWorldPosition);
        }

        private void SetPlayerCharWorldPosition(Vector3 playerWorldPosition)
        {
            _playerWorldPosition = playerWorldPosition;
            
            _playerCharView.transform.position = _playerWorldPosition;

            _eventBus.Dispatch(new PlayerCharPositionChangedEvent(_playerWorldPosition));
        }

        private void UpdateAnimation()
        {
            switch (_moveDirection.x)
            {
                case > 0:
                    _playerCharView.ToRightSide();
                    break;
                case < 0:
                    _playerCharView.ToLeftSide();
                    break;
            }

            if (ShouldPlayerNotMove)
            {
                _playerCharView.ToIdleState(_playerCharModel.HasProducts);
            }
            else
            {
                _playerCharView.ToWalkState(_playerCharModel.HasProducts);
            }
        }

        private Vector2 ClampMoveDirection(Vector2 moveDirection)
        {
            var playerPos = _playerCharView.transform.position;

            var resultMoveDirection = moveDirection;
            
            for (var i = 0; i < _detectorOffsets.Length; i++)
            {
                var detectorOffsetData = _detectorOffsets[i];
                if (CheckIsBlocked(playerPos, detectorOffsetData.offsets))
                {
                    var worldDirectionData = _detectorWorldDirections[detectorOffsetData.direction];

                    if (Vector3.Dot(worldDirectionData.worldDirection, resultMoveDirection) > 0)
                    {
                        resultMoveDirection = (worldDirectionData.worldDirectionToProject * Vector3.Dot(worldDirectionData.worldDirectionToProject, resultMoveDirection));//.normalized;
                    }
                }
            }
            
            return resultMoveDirection;
        }

        private bool CheckIsBlocked(Vector3 playerPos, Vector3[] offsets)
        {
            foreach (var offset in offsets)
            {
                var cellPos = _gridCalculator.WorldToCell(playerPos + offset);
                if (IsWalkable(cellPos) == false)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsWalkable(Vector2Int cellPos)
        {
            return _shopModel.IsOutOfShop(cellPos) == false
                   && _ownedCellsDataHolder.IsWalkableForPlayerChar(cellPos);
        }


        private void OnDrawGizmosHappened()
        {
            Gizmos.DrawCube(_playerCharView.transform.position + (Vector3)_moveDirection, Vector3.one * 0.2f);

            foreach (var valueTuple in _detectorOffsets)
            {
                foreach (var offsetValue in valueTuple.offsets)
                {
                    Gizmos.DrawCube(_playerCharView.transform.position + offsetValue, Vector3.one * 0.1f);
                }
            }
            
            //Gizmos.DrawCube(_playerCharView.transform.position + _gridCalculator.WorldCellXDirection * _halfCellSize, Vector3.one * 0.2f);
            //Gizmos.DrawCube(_playerCharView.transform.position + _gridCalculator.WorldCellYDirection * _halfCellSize, Vector3.one * 0.2f);
        }

        private void OnMovingVectorChangedEvent(MovingVectorChangedEvent e)
        {
            _moveDirection = new Vector3(e.Direction.x, -e.Direction.y);
        }
    }
}