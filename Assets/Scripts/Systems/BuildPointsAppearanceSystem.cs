using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Dto.ShopObjects;
using Holders;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using UnityEngine;
using Utils;

namespace Systems
{
    public class BuildPointsAppearanceSystem : ISystem
    {
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly BuildPointsDataHolderSo _buildPointsDataHolder = Instance.Get<BuildPointsDataHolderSo>();
        private readonly IUpgradeCostProvider _upgradeCostProvider = Instance.Get<IUpgradeCostProvider>();
        private readonly TruckPointsSettingsProviderSo _truckPointsSettingsProviderSo = Instance.Get<TruckPointsSettingsProviderSo>();
        
        private ShopModel _shopModel;
        private List<LinkedList<ShopObjectModelBase>> _shelfsByRow;
        
        private int CashDesksAmount => _shopModel.CashDesks.Count;
        private int TruckPointsAmount => _shopModel.TruckPoints.Count;
        private int ShelfsCount => _shopModel.Shelfs.Count;

        public void Start()
        {
            _shopModel = _shopModelHolder.ShopModel;

            PopulateObjectsAmount();
            UpdateBuildPoints();

            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void UpdateBuildPoints()
        {
            UpdateCashDeskBuildPoints();

            UpdateShelfBuildPoints();

            UpdateTruckGateBuildPoints();

            UpdateExpandShopPoints();
        }

        private void UpdateExpandShopPoints()
        {
            if (CashDesksAmount > 0 && ShelfsCount > 0 && TruckPointsAmount > 0 
                && _shopModel.ExpandPoints.Count < 2)
            {
                TryAddExpandXPoint();

                TryAddExpandYPoint();
            }
        }

        private void TryAddExpandXPoint()
        {
            var cellCoords = new Vector2Int(_shopModel.Size.x - 1, Constants.ExpandPointFreeCoord);

            if (_shopModel.ExpandPoints.Any(p => p.CellCoords == cellCoords)) return;
            
            var prevExpandXPoint = _shopModel.ExpandPoints.FirstOrDefault(p => p.CellCoords.y == Constants.ExpandPointFreeCoord);
            if (prevExpandXPoint != null)
            {
                _shopModel.RemoveBuildPoint(prevExpandXPoint.CellCoords);
            }

            var cost = _upgradeCostProvider.GetExpandXCost(_shopModel.Size.x);
            
            var expandXPoint = new BuildPointDto(BuildPointType.Expand, ShopObjectType.Undefined, cellCoords, cost);
            
            TryAddBuildPoint(expandXPoint);
        }

        private void TryAddExpandYPoint()
        {
            var cellCoords = new Vector2Int(Constants.ExpandPointFreeCoord, _shopModel.Size.y - 1);
            
            if (_shopModel.ExpandPoints.Any(p => p.CellCoords == cellCoords)) return;
            
            var prevExpandXPoint = _shopModel.ExpandPoints.FirstOrDefault(p => p.CellCoords.x == Constants.ExpandPointFreeCoord);
            if (prevExpandXPoint != null)
            {
                _shopModel.RemoveBuildPoint(prevExpandXPoint.CellCoords);
            }
            
            var cost = _upgradeCostProvider.GetExpandYCost(_shopModel.Size.y);
            
            var expandYPoint = new BuildPointDto(BuildPointType.Expand, ShopObjectType.Undefined, cellCoords, cost);
            
            TryAddBuildPoint(expandYPoint);
        }

        private void UpdateCashDeskBuildPoints()
        {
            if (CashDesksAmount <= 0 || ShelfsCount > CashDesksAmount)
            {
                var buildPointDto = _buildPointsDataHolder.GetCashDeskBuildPointData(CashDesksAmount);
                TryAddBuildPoint(buildPointDto);
            }
        }

        private void UpdateShelfBuildPoints()
        {
            if (CashDesksAmount <= 0) return;

            for (var rowIndex = 0; rowIndex <= _shelfsByRow.Count; rowIndex++)
            {
                var rowYCoord = _buildPointsDataHolder.RowIndexToYCoord(rowIndex);
                var shelfBuildPointsOnRow = _shopModel.GetShelfBuildPointsCountByRowYCoord(rowYCoord);

                if (shelfBuildPointsOnRow <= 0 && (rowIndex == 0 || _shelfsByRow[rowIndex - 1].Count > 0))
                {
                    var shelfsOnRowCount = rowIndex < _shelfsByRow.Count ? _shelfsByRow[rowIndex].Count : 0;

                    if (_buildPointsDataHolder.TryGetShelfBuildPointData(rowIndex, shelfsOnRowCount,
                            out var buildPointDto))
                    {
                        TryAddBuildPoint(buildPointDto);
                    }
                }
            }
        }

        private void UpdateTruckGateBuildPoints()
        {
            if (_shelfsByRow.Count > TruckPointsAmount 
                && TruckPointsAmount < _truckPointsSettingsProviderSo.TruckPointSettingsCount)
            {
                if (_buildPointsDataHolder.TryGetTruckGateBuildPointData(TruckPointsAmount, out var buildPointDto))
                {
                    TryAddBuildPoint(buildPointDto);
                }
            }
        }

        private void TryAddBuildPoint(BuildPointDto buildPointData)
        {
            if (_shopModel.HaveBuildPoint(buildPointData.CellCoords) == false
                && _shopModel.Size.x > buildPointData.CellCoords.x
                && _shopModel.Size.y > buildPointData.CellCoords.y +
                (buildPointData.BuildPointType == BuildPointType.BuildShopObject ? 1 : 0))
            {
                var buildPointModel = DataConverter.ToBuildPointModel(buildPointData);

                _shopModel.AddBuildPoint(buildPointModel);
            }
        }

        private void PopulateObjectsAmount()
        {
            var shopObjects = _shopModel.ShopObjects.Values.ToArray();

            _shelfsByRow = shopObjects
                .Where(s => s.ShopObjectType.IsShelf())
                .GroupBy(s => s.CellCoords.y, s => s)
                .OrderBy(g => g.Key)
                .Select(g => new LinkedList<ShopObjectModelBase>(g))
                .ToList();
        }

        private void Subscribe()
        {
            _shopModel.ShopObjectAdded += OnShopObjectAdded;
            _shopModel.ShopExpanded += OnShopExpanded;
        }

        private void Unsubscribe()
        {
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
            _shopModel.ShopExpanded -= OnShopExpanded;
        }

        private void OnShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            ConsiderShopObjectAdded(shopObjectModel);
            UpdateBuildPoints();
        }

        private void OnShopExpanded(Vector2Int _)
        {
            UpdateBuildPoints();
        }

        private void ConsiderShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            if (shopObjectModel.ShopObjectType.IsShelf())
            {
                ConsiderNewShelf(shopObjectModel);
            }
        }

        private void ConsiderNewShelf(ShopObjectModelBase shopObjectModel)
        {
            foreach (var shelfsInRow in _shelfsByRow)
            {
                if (shelfsInRow.First().CellCoords.y == shopObjectModel.CellCoords.y)
                {
                    shelfsInRow.AddLast(shopObjectModel);
                    return;
                }
            }

            var shelfsInNewRowList = new LinkedList<ShopObjectModelBase>();
            shelfsInNewRowList.AddFirst(shopObjectModel);
            
            _shelfsByRow.Add(shelfsInNewRowList);
        }
    }
}