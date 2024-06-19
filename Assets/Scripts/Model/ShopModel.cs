using System;
using System.Collections.Generic;
using Data;
using Model.BuildPoint;
using Model.ShopObjects;
using UnityEngine;

namespace Model
{
    public class ShopModel
    {
        public event Action<ShopObjectModelBase> ShopObjectAdded;
        public event Action<BuildPointModel> BuildPointAdded;
        public event Action<BuildPointModel> BuildPointRemoved;

        public Vector2Int Size;
        public WallType WallsType;
        public FloorType FloorsType;
        
        private readonly Dictionary<Vector2Int, ShopObjectModelBase> _shopObjects = new();
        private readonly Dictionary<Vector2Int, BuildPointModel> _buildPoints = new();

        public ShopModel(Vector2Int size, WallType wallsType, FloorType floorsType,
            IEnumerable<ShopObjectModelBase> shopObjects, IEnumerable<BuildPointModel> buildPoints)
        {
            Size = size;
            WallsType = wallsType;
            FloorsType = floorsType;

            SetShopObjects(shopObjects);
            SetBuildPoints(buildPoints);
        }

        public IReadOnlyDictionary<Vector2Int, ShopObjectModelBase> ShopObjects => _shopObjects;
        public IReadOnlyDictionary<Vector2Int, BuildPointModel> BuildPoints => _buildPoints;

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
    }
}