using System;
using UnityEngine;

namespace Data.Dto.ShopObjects
{
    [Serializable]
    public struct BuildPointDto
    {
        public ShopObjectType ShopObjectType;
        public Vector2Int CellCoords;
        public int MoneyToBuildLeft;
    }
}