using System;
using System.Collections.Generic;
using Data;
using Model.BuildPoint;
using Model.Customers;
using Model.ShopObjects;
using UnityEngine;

namespace Model
{
    public class ShopModel
    {
        public event Action<ShopObjectModelBase> ShopObjectAdded;
        public event Action<BuildPointModel> BuildPointAdded;
        public event Action<BuildPointModel> BuildPointRemoved;
        public event Action DoorsAdded;

        public readonly CustomersModel CustomersModel = new();
        
        private readonly Dictionary<Vector2Int, ShopObjectModelBase> _shopObjects = new();
        private readonly Dictionary<Vector2Int, BuildPointModel> _buildPoints = new();
        
        public WallType WallsType;
        public FloorType FloorsType;

        private Vector2Int _size;
        private (int Left, int Right)[] _doors;

        public ShopModel(Vector2Int size, WallType wallsType, FloorType floorsType,
            IEnumerable<ShopObjectModelBase> shopObjects, IEnumerable<BuildPointModel> buildPoints)
        {
            WallsType = wallsType;
            FloorsType = floorsType;

            SetSize(size);
            SetShopObjects(shopObjects);
            SetBuildPoints(buildPoints);
        }

        public Vector2Int Size => _size;
        public (int Left, int Right)[] Doors => _doors;

        public IReadOnlyDictionary<Vector2Int, ShopObjectModelBase> ShopObjects => _shopObjects;

        public IReadOnlyDictionary<Vector2Int, BuildPointModel> BuildPoints => _buildPoints;

        private void SetSize(Vector2Int size)
        {
            if (_size.Equals(size)) return;
            
            _size = size;

            UpdateDoors();
        }

        private void UpdateDoors()
        {
            var result = new List<(int, int)>(_size.x);
            var doorsLengthBefore = _doors?.Length ?? 0;

            for (var i = 0; i < 100; i++)
            {
                var doorsCoords = GetDoorsCoords(i);
                
                if (doorsCoords.Right < Size.x - 1)
                {
                    result.Add(doorsCoords);
                }

                if (doorsCoords.Left > _size.x)
                {
                    break;
                }
            }

            _doors = result.ToArray();
            
            if (_doors.Length > doorsLengthBefore)
            {
                DoorsAdded?.Invoke();
            }
        }

        public bool HaveBuildPoint(Vector2Int cellCoords)
        {
            return _buildPoints.ContainsKey(cellCoords);
        }

        public void AddBuildPoint(BuildPointModel buildPoint)
        {
            _buildPoints[buildPoint.CellCoords] = buildPoint;
            
            BuildPointAdded?.Invoke(buildPoint);
        }

        public bool TryGetTruckPoint(Vector2Int cellCoords, out TruckPointModel truckPointModel)
        {
            truckPointModel = null;
            
            if (ShopObjects.TryGetValue(cellCoords, out var shopObject)
                && shopObject.ShopObjectType == ShopObjectType.TruckPoint)
            {
                truckPointModel = (TruckPointModel)shopObject;
            }

            return truckPointModel != null;
        }
        
        public bool TryGetCashDesk(Vector2Int cellCoords, out CashDeskModel truckPointModel)
        {
            truckPointModel = null;
            
            if (ShopObjects.TryGetValue(cellCoords, out var shopObject)
                && shopObject.ShopObjectType == ShopObjectType.CashDesk)
            {
                truckPointModel = (CashDeskModel)shopObject;
            }

            return truckPointModel != null;
        }
        
        public bool TryGetShelfModel(Vector2Int cellCoords, out ShelfModel shelfModel)
        {
            shelfModel = null;
    
            if (ShopObjects.TryGetValue(cellCoords, out var shopObject)
                && shopObject.ShopObjectType.IsShelf())
            {
                shelfModel = (ShelfModel)shopObject;
            }

            return shelfModel != null;
        }

        public bool RemoveBuildPoint(Vector2Int cellCoords)
        {
            if (_buildPoints.TryGetValue(cellCoords, out var buildPointModel))
            {
                _buildPoints.Remove(cellCoords);
                
                BuildPointRemoved?.Invoke(buildPointModel);

                return true;
            }

            return false;
        }
        
        public bool AddShopObject(ShopObjectModelBase shopObject)
        {
            if (_shopObjects.TryAdd(shopObject.CellCoords, shopObject))
            {
                ShopObjectAdded?.Invoke(shopObject);
                
                return true;
            }

            return false;
        }

        public bool HaveDoorOn(int xCoord)
        {
            foreach (var doorCoords in _doors)
            {
                if (doorCoords.Left == xCoord || doorCoords.Right == xCoord)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsOutOfShop(Vector2Int cell)
        {
            return cell.x < 0 || cell.y < 0 || cell.x >= Size.x || cell.y >= Size.y;
        }

        private (int Left, int Right) GetDoorsCoords(int doorIndex)
        {
            return (GetDoorLeftPoint(doorIndex), GetDoorRightPoint(doorIndex));
        }

        private void SetShopObjects(IEnumerable<ShopObjectModelBase> shopObjects)
        {
            foreach (var shopObject in shopObjects)
            {
                _shopObjects[shopObject.CellCoords] = shopObject;
            }
        }

        private void SetBuildPoints(IEnumerable<BuildPointModel> buildPoints)
        {
            foreach (var buildPointModel in buildPoints)
            {
                _buildPoints[buildPointModel.CellCoords] = buildPointModel;
            }
        }

        private static int GetDoorLeftPoint(int doorIndex)
        {
            return 3 + doorIndex * 8;
        }
        
        private static int GetDoorRightPoint(int doorIndex)
        {
            return GetDoorLeftPoint(doorIndex) + 1;
        }
    }
}