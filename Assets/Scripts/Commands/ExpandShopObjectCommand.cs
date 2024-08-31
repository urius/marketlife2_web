using Data;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Model.SpendPoints;
using UnityEngine;
using Utils;

namespace Commands
{
    public struct ExpandShopObjectCommand : ICommand<BuildPointModel>
    {
        public void Execute(BuildPointModel expandPoint)
        {
            if (expandPoint.BuildPointType != BuildPointType.Expand)
            {
                Debug.LogWarning(
                    $"trying to execute {nameof(ExpandShopObjectCommand)} with unsupported {nameof(expandPoint.BuildPointType)}: {expandPoint.BuildPointType}");
                return;
            }
            
            var shopModel = Instance.Get<IShopModelHolder>().ShopModel;
            var expandPointCoords = expandPoint.CellCoords;
            
            shopModel.RemoveBuildPoint(expandPointCoords);

            var expandX = ExpandShopHelper.IsExpandX(expandPointCoords) ? Constants.ExpandCellsAmount : 0;
            var expandY = ExpandShopHelper.IsExpandY(expandPointCoords) ? Constants.ExpandCellsAmount : 0;

            shopModel.Expand(new Vector2Int(expandX, expandY));
        }
    }
}