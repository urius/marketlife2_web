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

                var unlockLevel = GetExpandLevelByExpandPoint(expandPoint);
                
                return playerModel.Level >= unlockLevel;
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

            return 2 * GetTotalExpandsCount(currentXShopSize, initialXShopSize) - 1 + Constants.MinLevelForShopExpand - 1;
        }

        public static int GetYExpandLevel(int currentXShopSize)
        {
            var defaultPlayerDataHolder = Instance.Get<DefaultPlayerDataHolderSo>();
            var initialXShopSize = defaultPlayerDataHolder.DefaultPlayerData.ShopData.Size.x;

            return 2 * GetTotalExpandsCount(currentXShopSize, initialXShopSize) + Constants.MinLevelForShopExpand - 1;
        }
        
        private static int GetTotalExpandsCount(int currentShopSize, int initialShopSize)
        {
            var sizeAfterExpand = currentShopSize + Constants.ExpandCellsAmount;
            var deltaSizeAfterExpand = sizeAfterExpand - initialShopSize;
            var deltaLevelsToExpand = deltaSizeAfterExpand / Constants.ExpandCellsAmount;

            return deltaLevelsToExpand;
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