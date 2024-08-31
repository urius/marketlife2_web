using System;
using UnityEngine;

namespace Data.Dto.ShopObjects
{
    [Serializable]
    public struct BuildPointDto
    {
        public BuildPointType BuildPointType;
        public ShopObjectType ShopObjectType;
        public Vector2Int CellCoords;
        public int MoneyToBuildLeft;
        
        public BuildPointDto(BuildPointType buildPointType, ShopObjectType shopObjectType, Vector2Int cellCoords, int moneyToBuildLeft)
        {
            BuildPointType = buildPointType;
            ShopObjectType = shopObjectType;
            CellCoords = cellCoords;
            MoneyToBuildLeft = moneyToBuildLeft;
        }
    }
}