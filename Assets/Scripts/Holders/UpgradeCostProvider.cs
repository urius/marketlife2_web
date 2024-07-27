using Infra.Instance;
using Model.ShopObjects;

namespace Holders
{
    public class UpgradeCostProvider : IUpgradeCostProvider
    {
        private readonly TruckPointsSettingsProviderSo _truckPointsSettingsProviderSo = Instance.Get<TruckPointsSettingsProviderSo>();
        private readonly BuildPointsDataHolderSo _buildPointsDataHolder = Instance.Get<BuildPointsDataHolderSo>();
        
        
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
    }

    public interface IUpgradeCostProvider
    {
        public int GetTruckPointUpgradeCost(TruckPointModel truckPointModel);
    }
}