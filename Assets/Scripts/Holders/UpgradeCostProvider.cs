using Data;
using Infra.Instance;
using Model.ShopObjects;

namespace Holders
{
    public class UpgradeCostProvider : IUpgradeCostProvider
    {
        private readonly BuildPointsDataHolderSo _buildPointsDataHolder = Instance.Get<BuildPointsDataHolderSo>();
        private readonly IShelfUpgradeSettingsProvider _shelfUpgradeSettingsProvider = Instance.Get<IShelfUpgradeSettingsProvider>();
        
        public int GetTruckPointUpgradeCost(TruckPointModel truckPointModel)
        {
            if (truckPointModel.CanUpgrade())
            {
                var tpIndex= _buildPointsDataHolder.GetTruckPointIndexByCoords(truckPointModel.CellCoords);
                if (_buildPointsDataHolder.TryGetTruckGateBuildPointData(tpIndex, out var buildPointDto))
                {
                    return (truckPointModel.UpgradesCount + 1) * buildPointDto.MoneyToBuildLeft;
                }
            }

            return -1;
        }

        public int GetShelfUpgradeCost(ShelfModel shelfModel)
        {
            var rowIndex = _buildPointsDataHolder.YCoordToRowIndex(shelfModel.CellCoords.y);
            var shelfCoords = shelfModel.CellCoords;
            var cellOffset = Constants.ShopObjectRelativeToBuildPointOffset;

            for (var index = 0; index < 100; index++)
            {
                if (_buildPointsDataHolder.TryGetShelfBuildPointData(rowIndex, index, out var tempBuildPointDto)
                    && (tempBuildPointDto.CellCoords + cellOffset) == shelfCoords
                    && _shelfUpgradeSettingsProvider.CanUpgradeTo(shelfModel.ShopObjectType, shelfModel.UpgradeIndex + 1))
                {
                    var shelfBuildCost = tempBuildPointDto.MoneyToBuildLeft;
                    return shelfBuildCost * (shelfModel.UpgradeIndex + 1);
                }
            }

            return -1;
        }
    }

    public interface IUpgradeCostProvider
    {
        public int GetTruckPointUpgradeCost(TruckPointModel truckPointModel);
        public int GetShelfUpgradeCost(ShelfModel shelfModel);
    }
}