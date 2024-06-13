using System;
using Data;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Model.BuildPoint;
using Model.ShopObjects;
using UnityEngine;

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
            var buildCoords = buildPoint.CellCoords + Vector2Int.left;;
            
            switch (shopObjectType)
            {
                case ShopObjectType.CashDesk:
                    result = new CashDeskModel(buildCoords);
                    break;
                case ShopObjectType.TruckPoint:
                    result = new TruckPointModel(buildCoords);
                    break;
                default:
                    if (shopObjectType.IsShelf())
                    {
                        var shelfSettingsProvider = Instance.Get<IShelfSettingsProvider>();
                        
                        shelfSettingsProvider.TryGetShelfSetting(shopObjectType, 0, out var shelfSettings);
                        
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
    }
}