using Data;
using Holders;
using Infra.Instance;
using Model.SpendPoints;
using UnityEngine;

namespace Utils
{
    public static class ExpandShopHelper
    {
        public static bool IsExpandUnlocked(BuildPointModel expandPoint)
        {
            if (expandPoint.BuildPointType == BuildPointType.Expand)
            {
                var playerModelHolder = Instance.Get<IPlayerModelHolder>();
                
                var playerModel = playerModelHolder.PlayerModel;
                var shopModel = playerModel.ShopModel;
                
                if (IsExpandX(expandPoint.CellCoords))
                {
                    var targetLevel = GetXExpandLevel(shopModel.Size.x);
                    return playerModel.Level >= targetLevel;
                }

                if (IsExpandY(expandPoint.CellCoords))
                {
                    var targetLevel = GetYExpandLevel(shopModel.Size.y);
                    return playerModel.Level >= targetLevel;
                }
            }

            return false;
        }

        public static int GetExpandLevelByExpandPoint(BuildPointModel expandPoint)
        {
            if (expandPoint.BuildPointType == BuildPointType.Expand)
            {
                var playerModelHolder = Instance.Get<IPlayerModelHolder>();
                
                var playerModel = playerModelHolder.PlayerModel;
                var shopModel = playerModel.ShopModel;
                
                if (IsExpandX(expandPoint.CellCoords))
                {
                    return GetXExpandLevel(shopModel.Size.x);
                }

                if (IsExpandY(expandPoint.CellCoords))
                {
                    return GetYExpandLevel(shopModel.Size.y);
                }
            }

            return -1;
        }

        public static int GetXExpandLevel(int currentXShopSize)
        {
            var defaultPlayerDataHolder = Instance.Get<DefaultPlayerDataHolderSo>();
            var initialXShopSize = defaultPlayerDataHolder.DefaultPlayerData.ShopData.Size.x;

            return GetExpandLevel(currentXShopSize, initialXShopSize);
        }

        public static int GetYExpandLevel(int currentXShopSize)
        {
            var defaultPlayerDataHolder = Instance.Get<DefaultPlayerDataHolderSo>();
            var initialXShopSize = defaultPlayerDataHolder.DefaultPlayerData.ShopData.Size.x;

            return GetExpandLevel(currentXShopSize, initialXShopSize);
        }
        
        private static int GetExpandLevel(int currentShopSize, int initialShopSize)
        {
            var sizeAfterExpand = currentShopSize + Constants.ExpandCellsAmount;
            var deltaSizeAfterExpand = sizeAfterExpand - initialShopSize;
            var deltaLevelsToExpand = deltaSizeAfterExpand / Constants.ExpandCellsAmount;
            
            return deltaLevelsToExpand + Constants.MinLevelForShopExpand - 1;
        }

        public static bool IsExpandX(Vector2Int buildPointCellCoords)
        {
            return buildPointCellCoords.y == Constants.ExpandPointFreeCoord;
        }
        
        public static bool IsExpandY(Vector2Int buildPointCellCoords)
        {
            return buildPointCellCoords.x == Constants.ExpandPointFreeCoord;
        }
    }
}