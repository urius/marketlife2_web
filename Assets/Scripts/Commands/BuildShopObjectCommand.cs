using System;
using System.Linq;
using Data;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Model.BuildPoint;
using Model.People;
using Model.ShopObjects;

namespace Commands
{
    public struct BuildShopObjectCommand : ICommand<BuildPointModel>
    {
        public void Execute(BuildPointModel buildPoint)
        {
            var shopModel = Instance.Get<IShopModelHolder>().ShopModel;
            
            shopModel.RemoveBuildPoint(buildPoint.CellCoords);
                    
            var shopObject = GetShopObjectByBuildPoint(buildPoint);
            shopModel.AddShopObject(shopObject);
        }
        
        private ShopObjectModelBase GetShopObjectByBuildPoint(BuildPointModel buildPoint)
        {
            ShopObjectModelBase result;
            
            var shopObjectType = buildPoint.ShopObjectType;
            var buildCoords = buildPoint.CellCoords + Constants.ShopObjectRelativeToBuildPointOffset;
            
            switch (shopObjectType)
            {
                case ShopObjectType.CashDesk:
                    result = new CashDeskModel(buildCoords);
                    break;
                case ShopObjectType.TruckPoint:
                    if (TryGetTruckPointsSettings(out var truckPointsSettings))
                    {
                        result = new TruckPointModel(buildCoords, truckPointsSettings,
                            truckPointsSettings.Products.Take(TruckPointModel.DefaultProductBoxesAmount).ToArray(),
                            upgradesCount: 0,
                            Constants.TruckArrivingDuration + 1,
                            Array.Empty<TruckPointStaffCharModel>());
                    }
                    else
                    {
                        throw new NotSupportedException(
                            $"{nameof(GetShopObjectByBuildPoint)}: unable to provide truck point setting for build point {buildPoint.CellCoords}");
                    }
                    break;
                default:
                    if (shopObjectType.IsShelf())
                    {
                        var shelfSettingsProvider = Instance.Get<IShelfUpgradeSettingsProvider>();
                        
                        shelfSettingsProvider.TryGetShelfUpgradeSetting(shopObjectType, 0, out var shelfSettings);
                        
                        result = new ShelfModel(buildCoords, shopObjectType, shelfSettings.SlotsAmount);
                    }
                    else
                    {
                        throw new NotImplementedException(
                            $"{nameof(GetShopObjectByBuildPoint)}: unknown shopObjectType {shopObjectType}");
                    }
                    break;
            }

            return result;
        }

        private bool TryGetTruckPointsSettings(out TruckPointSetting setting)
        {
            var shopModel = Instance.Get<IShopModelHolder>().ShopModel;
            var truckPointsSettingsProvider = Instance.Get<TruckPointsSettingsProviderSo>();

            var truckPointsAmount =
                shopModel.ShopObjects.Values.Count(o => o.ShopObjectType == ShopObjectType.TruckPoint);

            var result =
                truckPointsSettingsProvider.TryGetSettingByTruckPointIndex(truckPointsAmount,
                    out var truckPointSetting);

            setting = truckPointSetting;

            return result;
        }
    }
}