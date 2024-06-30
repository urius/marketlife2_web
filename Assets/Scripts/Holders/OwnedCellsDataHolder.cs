using System;
using System.Collections.Generic;
using Model.ShopObjects;
using UnityEngine;

namespace Holders
{
    public class OwnedCellsDataHolder : IOwnedCellsDataHolder
    {
        private readonly Dictionary<Vector2Int, OwnedCellData> _ownedCellDataByCoords = new();
        private readonly LinkedList<OwnedCellData> _ownedCellDataList = new();
        
        public bool RegisterShopObject(ShopObjectModelBase shopObjectModel, Vector2Int[] ownedCells)
        {
            if (CheckShopObjectExist(shopObjectModel) == false)
            {
                var ownedCellData = new OwnedCellByShopObjectData(shopObjectModel, ownedCells);
                _ownedCellDataList.AddLast(ownedCellData);
                
                foreach (var ownedCell in ownedCells)
                {
                    if (_ownedCellDataByCoords.ContainsKey(ownedCell))
                    {
                        Debug.LogWarning($"Overriding already owned by another shop object cell: {ownedCell}");
                    }

                    _ownedCellDataByCoords[ownedCell] = ownedCellData;
                }

                return true;
            }

            return false;
        }

        public void UnregisterShopObject(ShopObjectModelBase shopObjectModel)
        {
            if (TryGetShopObjectOwner(shopObjectModel.CellCoords, out var ownerData))
            {
                foreach (var ownedCell in ownerData.OwnedCells)
                {
                    _ownedCellDataByCoords.Remove(ownedCell);
                }

                _ownedCellDataList.Remove(ownerData);
            }
        }

        public bool IsOwnedByShopObject(Vector2Int cellCoords)
        {
            return _ownedCellDataByCoords.ContainsKey(cellCoords);
        }

        public bool TryGetShopObjectOwner(Vector2Int shopObjectCellCoords, out OwnedCellByShopObjectData ownerData)
        {
            ownerData = null;
            
            if (_ownedCellDataByCoords.TryGetValue(shopObjectCellCoords, out var ownedCellData)
                && ownedCellData.OwnedCellDataObjectType == OwnedCellDataObjectType.ShopObject)
            {
                ownerData = ((OwnedCellByShopObjectData)ownedCellData);
            }

            return ownerData != null;
        }
        
        public Vector2Int[] GetShopObjectOwnedCells(ShopObjectModelBase shopObjectModel)
        {
            if (TryGetShopObjectOwner(shopObjectModel.CellCoords, out var ownerData))
            {
                return ownerData.OwnedCells;
            }

            return Array.Empty<Vector2Int>();
        }

        public bool IsWalkableForPlayerChar(Vector2Int cellCoords)
        {
            return IsOwnedByShopObject(cellCoords) == false;
        }

        public bool IsWalkableForCustomerChar(Vector2Int cellCoords)
        {
            return IsOwnedByShopObject(cellCoords) == false;
        }

        private bool CheckShopObjectExist(ShopObjectModelBase shopObjectModel)
        {
            foreach (var ownedCellData in _ownedCellDataList)
            {
                if (ownedCellData.OwnedCellDataObjectType == OwnedCellDataObjectType.ShopObject
                    && ((OwnedCellByShopObjectData)ownedCellData).ShopObjectModel == shopObjectModel)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public interface IOwnedCellsDataHolder
    {
        public bool RegisterShopObject(ShopObjectModelBase shopObjectModel, Vector2Int[] ownedCells);
        public bool IsOwnedByShopObject(Vector2Int cellCoords);
        public bool TryGetShopObjectOwner(Vector2Int shopObjectCellCoords, out OwnedCellByShopObjectData ownedData);
        public Vector2Int[] GetShopObjectOwnedCells(ShopObjectModelBase shopObjectModel);
        public bool IsWalkableForPlayerChar(Vector2Int cellCoords);
        public bool IsWalkableForCustomerChar(Vector2Int cellCoords);
        public void UnregisterShopObject(ShopObjectModelBase shopObjectModel);
    }

    public abstract class OwnedCellData
    {
        public abstract OwnedCellDataObjectType OwnedCellDataObjectType { get; }
    }

    public class OwnedCellByShopObjectData : OwnedCellData
    {
        public readonly ShopObjectModelBase ShopObjectModel;

        public OwnedCellByShopObjectData(ShopObjectModelBase shopObjectModel, Vector2Int[] ownedCells) 
        {
            ShopObjectModel = shopObjectModel;
            OwnedCells = ownedCells;
        }

        public Vector2Int[] OwnedCells { get; }

        public override OwnedCellDataObjectType OwnedCellDataObjectType => OwnedCellDataObjectType.ShopObject;
    }

    public enum OwnedCellDataObjectType
    {
        ShopObject,
        Man,
    }
}