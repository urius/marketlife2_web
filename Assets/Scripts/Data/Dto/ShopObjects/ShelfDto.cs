using System;
using UnityEngine;

namespace Data.Dto.ShopObjects
{
    [Serializable]
    public struct ShelfDto
    {
        public ShopObjectType ShelfType;
        public Vector2Int CellCoords;
        public int UpgradeIndex;
        public int[] Products;

        public ShelfDto(
            ShopObjectType shelfType,
            Vector2Int cellCoords,
            int upgradeIndex,
            int[] products)
        {
            ShelfType = shelfType;
            CellCoords = cellCoords;
            UpgradeIndex = upgradeIndex;
            Products = products;
        }
    }
}