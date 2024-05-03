using System.Collections.Generic;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;
using Utils;

namespace View.Game.People
{
    public class PlayerCharMediator : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private static readonly Vector2Int CellPosRight = new(1, 0);
        private static readonly Vector2Int CellPosLeft = new(-1, 0);
        private static readonly Vector2Int CellPosDown = new(0, 1);
        private static readonly Vector2Int CellPosUp = new(0, -1);

        private Dictionary<Vector2Int, (Vector3 worldDirection, Vector3 worldDirectionToProject)> _detectorWorldDirections;

        private ManView _playerCharView;
        private Vector2 _moveDirection;
        private (Vector2Int direction, Vector3[] offsets)[] _detectorOffsets;

        protected override void MediateInternal()
        {
            FillDetectorOffsets();
            
            var playerCharViewGo = InstantiatePrefab(PrefabKey.Man);

            _playerCharView = playerCharViewGo.GetComponent<ManView>();

            Subscribe();
        }

        private void FillDetectorOffsets()
        {
            var cellPrimaryOffset = _gridCalculator.CellSize.x * 0.3f;
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
            if (_moveDirection == Vector2.zero) return;
            
            const float speed = 0.02f;
            var transform = _playerCharView.transform;

            var clampedMoveDirection = ClampMoveDirection(_moveDirection);

            var delta = clampedMoveDirection * speed;
            var pos = transform.position + (Vector3)delta;

            transform.position = pos;
        }

        private Vector2 ClampMoveDirection(Vector2 moveDirection)
        {
            var playerPos = _playerCharView.transform.position;

            var resultMoveDirection = moveDirection;
            
            foreach (var valueTuple in _detectorOffsets)
            {
                if (CheckIsBlocked(playerPos, valueTuple.offsets))
                {
                    var worldDirectionData = _detectorWorldDirections[valueTuple.direction];

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
            //todo check from model
            if (cellPos.x < 0 || cellPos.y < 0) return false;
            if (cellPos == Vector2Int.one) return false;

            return true;
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
            _moveDirection = new Vector3(e.Direction.x, -e.Direction.y).normalized;
        }
    }
}