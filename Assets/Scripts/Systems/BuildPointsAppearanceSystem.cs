using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Dto.ShopObjects;
using Holders;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using Utils;

namespace Systems
{
    public class BuildPointsAppearanceSystem : ISystem
    {
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly BuildPointsDataHolderSo _buildPointsDataHolder = Instance.Get<BuildPointsDataHolderSo>();
        
        private ShopModel _shopModel;
        private List<LinkedList<ShopObjectModelBase>> _shelfsByRow;
        private int _cashDesksAmount;
        private int _truckPointsAmount;

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
            if (_cashDesksAmount <= 0)
            {
                var buildPointDto = _buildPointsDataHolder.GetCashDeskBuildPointData(0);
                TryAddBuildPoint(buildPointDto);
                return;
            }

            if (_cashDesksAmount > 0)
            {
                var shelfRowsCount = _shelfsByRow.Count;
                if (shelfRowsCount <= 0)
                {
                    if (_buildPointsDataHolder.TryGetShelfBuildPointData(_shelfsByRow.Count, 0, out var buildPointDto))
                    {
                        TryAddBuildPoint(buildPointDto);
                    }
                }
                else
                {
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

                if (shelfRowsCount > _truckPointsAmount)
                {
                    if (_buildPointsDataHolder.TryGetTruckGateBuildPointData(_truckPointsAmount, out var buildPointDto))
                    {
                        TryAddBuildPoint(buildPointDto);
                    }
                }
            }
        }

        private void TryAddBuildPoint(BuildPointDto buildPointData)
        {
            if (_shopModel.HaveBuildPoint(buildPointData.CellCoords) == false
                && _shopModel.Size.x > buildPointData.CellCoords.x
                && _shopModel.Size.y > buildPointData.CellCoords.y + 1)
            {
                var buildPointModel = DataConverter.ToBuildPointModel(buildPointData);
                
                _shopModel.AddBuildPoint(buildPointModel);
            }
        }

        private void PopulateObjectsAmount()
        {
            var shopObjects = _shopModel.ShopObjects.Values.ToArray();

            _cashDesksAmount = shopObjects.Count(o => o.ShopObjectType == ShopObjectType.CashDesk);

            _shelfsByRow = shopObjects
                .Where(s => s.ShopObjectType.IsShelf())
                .GroupBy(s => s.CellCoords.y, s => s)
                .OrderBy(g => g.Key)
                .Select(g => new LinkedList<ShopObjectModelBase>(g))
                .ToList();
            
            _truckPointsAmount = shopObjects.Count(o => o.ShopObjectType == ShopObjectType.TruckPoint);
        }

        private void Subscribe()
        {
            _shopModel.ShopObjectAdded += OnShopObjectAdded;
        }

        private void Unsubscribe()
        {
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
        }

        private void OnShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            ConsiderShopObjectAdded(shopObjectModel);
            UpdateBuildPoints();
        }

        private void ConsiderShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            if (shopObjectModel.ShopObjectType.IsShelf())
            {
                ConsiderNewShelf(shopObjectModel);
                return;
            }
            
            switch (shopObjectModel.ShopObjectType)
            {
                case ShopObjectType.CashDesk:
                    _cashDesksAmount++;
                    break;
                case ShopObjectType.TruckPoint:
                    _truckPointsAmount++;
                    break;
                default:
                    throw new NotImplementedException(
                        $"{nameof(OnShopObjectAdded)}: unsupported {nameof(shopObjectModel.ShopObjectType)} = {shopObjectModel.ShopObjectType}");
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