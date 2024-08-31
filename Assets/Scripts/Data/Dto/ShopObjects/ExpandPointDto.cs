using System;
using UnityEngine;

namespace Data.Dto.ShopObjects
{
    [Serializable]
    public struct ExpandPointDto
    {
        public Vector2Int CellCoords;
        public int MoneyToBuildLeft;
    }
}