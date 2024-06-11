using System.Collections.Generic;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
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

        private Dictionary<Vector2Int, (Vector3 worldDirection, Vector3 worldDirectionToProject)> _detectorWorldDirections;

        private ManView _playerCharView;
        private Vector2 _moveDirection;
        private Vector2Int _cellCoords;
        private (Vector2Int direction, Vector3[] offsets)[] _detectorOffsets;
        private Vector3 _playerWorldPosition;
        private DynamicViewSortingLogic _sortingLogic;

        protected override void MediateInternal()
        {
            FillDetectorOffsets();
            
            _playerCharView = TargetTransform.GetComponent<ManView>();
            _playerWorldPosition = _playerCharView.transform.position;

            _sortingLogic = new DynamicViewSortingLogic(_playerCharView, _ownedCellsDataHolder);
            _sortingLogic.UpdateSorting(_cellCoords);
            
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
            _updatesProvider.FixedUpdateHappened += OnFixedUpdateHappened;

            DebugDrawGizmosDispatcher.DrawGizmosHappened += OnDrawGizmosHappened;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<MovingVectorChangedEvent>(OnMovingVectorChangedEvent);
            _updatesProvider.FixedUpdateHappened -= OnFixedUpdateHappened;
            
            DebugDrawGizmosDispatcher.DrawGizmosHappened -= OnDrawGizmosHappened;
        }

        private void OnFixedUpdateHappened()
        {
            ProcessMove();
            CheckCellCoords();
        }

        private void CheckCellCoords()
        {
            if (_moveDirection == Vector2.zero) return;
            
            var newCellCoords = _gridCalculator.WorldToCell(_playerWorldPosition);
            
            if (newCellCoords != _cellCoords)
            {
                _cellCoords = newCellCoords;
                
                _sortingLogic.UpdateSorting(_cellCoords);
                
                _eventBus.Dispatch(new RequestPlayerCellChangeEvent(_cellCoords));
            }
        }

        private void ProcessMove()
        {
            if (_moveDirection == Vector2.zero) return;
            
            const float speed = 0.04f;

            var clampedMoveDirection = ClampMoveDirection(_moveDirection);

            var delta = clampedMoveDirection * speed;
            _playerWorldPosition += (Vector3)delta;

            _playerCharView.transform.position = _playerWorldPosition;

            _eventBus.Dispatch(new PlayerCharPositionChangedEvent(_playerWorldPosition));
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
                        resultMoveDirection = (worldDirectionData.worldDirectionToProject * Vector3.Dot(worldDirectionData.worldDirectionToProject, resultMoveDirection)).normalized;
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
            return _ownedCellsDataHolder.IsWalkableForPlayerChar(cellPos);
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