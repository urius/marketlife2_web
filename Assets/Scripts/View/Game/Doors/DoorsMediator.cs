using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using Model;
using UnityEngine;
using Utils;

namespace View.Game.Doors
{
    public class DoorsMediator : MediatorBase
    {
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private readonly List<DoorsView> _doorsViews = new();
        private readonly List<int> _doorsOpenStates = new();
        
        private ShopModel _shopModel;
        private PlayerCharModel _playerCharModel;
        private int _currentDoorIndex = -1;

        protected override void MediateInternal()
        {
            _shopModel = _shopModelHolder.ShopModel;
            _playerCharModel = _playerModelHolder.PlayerCharModel;
            
            DisplayDoors();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            _doorsViews.ForEach(Destroy);
        }

        private void Subscribe()
        {
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
        }

        private void Unsubscribe()
        {            
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
        }

        private void OnGameplayFixedUpdate()
        {
            if (_shopModel.Doors.Length <= 0) return;

            _currentDoorIndex++;
            if (_currentDoorIndex >= _shopModel.Doors.Length)
            {
                _currentDoorIndex = 0;
            }

            UpdateDoorOpenState(_currentDoorIndex);
            
            ProcessDoorsOpenClose();
        }

        private void ProcessDoorsOpenClose()
        {
            for (var i = 0; i < _doorsOpenStates.Count; i++)
            {
                var doorOpenState = _doorsOpenStates[i];
                if (doorOpenState == 1)
                {
                    if (_doorsViews[i].ProceedCloseDoors() >= 1)
                    {
                        _doorsOpenStates[i] = 2;
                    }
                }
                else if (doorOpenState == -1)
                {
                    if (_doorsViews[i].ProceedOpenDoors() <= -1)
                    {
                        _doorsOpenStates[i] = -2;
                    }
                }
            
            }
        }

        private void UpdateDoorOpenState(int doorIndex)
        {
            var doorData = _shopModel.Doors[doorIndex];
            
            var hasAnyManNearDoor = HasManOnCell(new Vector2Int(doorData.Left, 0))
                                    || HasManOnCell(new Vector2Int(doorData.Left, -1))
                                    || HasManOnCell(new Vector2Int(doorData.Right, 0))
                                    || HasManOnCell(new Vector2Int(doorData.Right, -1));

            if (hasAnyManNearDoor && _doorsOpenStates[doorIndex] >= 0)
            {
                _doorsOpenStates[doorIndex] = -1;
            }
            else if (!hasAnyManNearDoor && _doorsOpenStates[doorIndex] <= 0)
            {
                _doorsOpenStates[doorIndex] = 1;
            }
        }

        private bool HasManOnCell(Vector2Int cell)
        {
            return _shopModel.CustomersModel.HaveCustomerOnCell(cell)
                   || _playerCharModel.CellPosition == cell;
        }

        private void DisplayDoors()
        {
            if (_shopModel.Doors.Length <= _doorsViews.Count) return;
            
            _doorsViews.Capacity = _shopModel.Doors.Length;
            _doorsOpenStates.Capacity = _doorsViews.Capacity;
            
            for (var i = _doorsViews.Count; i < _shopModel.Doors.Length; i++)
            {
                CreateDoor(_shopModel.Doors[i].Left);
            }
        }

        private void CreateDoor(int leftDoorXCellPosition)
        {
            var doorsView = InstantiatePrefab<DoorsView>(PrefabKey.Doors);
            var worldPosition = _gridCalculator.GetCellCenterWorld(new Vector2Int(leftDoorXCellPosition, -1));
            doorsView.transform.position = worldPosition;

            _doorsViews.Add(doorsView);
            _doorsOpenStates.Add(0);
        }
    }
}