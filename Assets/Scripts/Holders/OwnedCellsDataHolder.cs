using System;
using System.Collections.Generic;
using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using Model.ShopObjects;
using UnityEngine;

namespace Holders
{
    public class OwnedCellsDataHolder : IOwnedCellsDataHolder
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private readonly Dictionary<Vector2Int, OwnedCellsByShopObjectData> _ownedCellDataByCoords = new();
        private readonly LinkedList<OwnedCellsByShopObjectData> _ownedCellDataList = new();
        
        public bool RegisterShopObject(ShopObjectModelBase shopObjectModel, Vector2Int[] ownedCells)
        {
            if (CheckShopObjectExist(shopObjectModel) == false)
            {
                var ownedCellData = new OwnedCellsByShopObjectData(shopObjectModel, ownedCells);
                _ownedCellDataList.AddLast(ownedCellData);
                
                foreach (var ownedCell in ownedCells)
                {
                    if (_ownedCellDataByCoords.ContainsKey(ownedCell))
                    {
                        Debug.LogWarning($"Overriding already owned by another shop object cell: {ownedCell}");
                    }

                    _ownedCellDataByCoords[ownedCell] = ownedCellData;
                }
                
                _eventBus.Dispatch(new ShopObjectCellsRegisteredEvent(shopObjectModel, ownedCells));

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
            else
            {
                Debug.LogWarning($"Can't Unregister Shop Object {shopObjectModel.ShopObjectType} on cell {shopObjectModel.CellCoords}");
            }
        }

        public bool IsOwnedByShopObject(Vector2Int cellCoords)
        {
            return _ownedCellDataByCoords.ContainsKey(cellCoords);
        }

        public bool TryGetShopObjectOwner(Vector2Int shopObjectCellCoords, out OwnedCellsByShopObjectData ownerData)
        {
            ownerData = default;
            
            if (_ownedCellDataByCoords.TryGetValue(shopObjectCellCoords, out var ownedCellData))
            {
                ownerData = ownedCellData;
                return true;
            }

            return false;
        }

        public bool TryGetCashDesk(Vector2Int shopObjectCellCoords, out CashDeskModel cashDeskModel)
        {
            cashDeskModel = null;

            if (_ownedCellDataByCoords.TryGetValue(shopObjectCellCoords, out var ownedCellData)
                && ownedCellData.ShopObjectModel.ShopObjectType == ShopObjectType.CashDesk)
            {
                cashDeskModel = (CashDeskModel)ownedCellData.ShopObjectModel;
            }
            
            return cashDeskModel != null;
        }
        
        public bool TryGetShelf(Vector2Int shopObjectCellCoords, out ShelfModel shelfModel)
        {
            shelfModel = null;

            if (_ownedCellDataByCoords.TryGetValue(shopObjectCellCoords, out var ownedCellData)
                && ownedCellData.ShopObjectModel.ShopObjectType.IsShelf())
            {
                shelfModel = (ShelfModel)ownedCellData.ShopObjectModel;
            }
    
            return shelfModel != null;
        }
        
        public bool TryGetTruckPoint(Vector2Int shopObjectCellCoords, out TruckPointModel truckPointModel)
        {
            truckPointModel = null;

            if (_ownedCellDataByCoords.TryGetValue(shopObjectCellCoords, out var ownedCellData)
                && ownedCellData.ShopObjectModel.ShopObjectType == ShopObjectType.TruckPoint)
            {
                truckPointModel = (TruckPointModel)ownedCellData.ShopObjectModel;
            }
    
            return truckPointModel != null;
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

        public bool IsWalkableForBotChar(Vector2Int cellCoords)
        {
            return IsOwnedByShopObject(cellCoords) == false;
        }

        private bool CheckShopObjectExist(ShopObjectModelBase shopObjectModel)
        {
            foreach (var ownedCellData in _ownedCellDataList)
            {
                if (ownedCellData.ShopObjectModel == shopObjectModel)
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
        public bool TryGetShopObjectOwner(Vector2Int shopObjectCellCoords, out OwnedCellsByShopObjectData ownedData);
        public bool TryGetCashDesk(Vector2Int shopObjectCellCoords, out CashDeskModel cashDeskModel);
        public bool TryGetTruckPoint(Vector2Int nearCell, out TruckPointModel oTruckPointModel);
        public bool TryGetShelf(Vector2Int shopObjectCellCoords, out ShelfModel shelfModel);
        public Vector2Int[] GetShopObjectOwnedCells(ShopObjectModelBase shopObjectModel);
        public bool IsWalkableForPlayerChar(Vector2Int cellCoords);
        public bool IsWalkableForBotChar(Vector2Int cellCoords);
        public void UnregisterShopObject(ShopObjectModelBase shopObjectModel);
    }

    public struct OwnedCellsByShopObjectData
    {
        public readonly ShopObjectModelBase ShopObjectModel;

        public OwnedCellsByShopObjectData(ShopObjectModelBase shopObjectModel, Vector2Int[] ownedCells) 
        {
            ShopObjectModel = shopObjectModel;
            OwnedCells = ownedCells;
        }

        public Vector2Int[] OwnedCells { get; }
    }
}