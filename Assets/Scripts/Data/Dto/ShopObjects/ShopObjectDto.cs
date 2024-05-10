using System;
using UnityEngine;

namespace Data.Dto.ShopObjects
{
    [Serializable]
    public struct ShopObjectDto
    {
        public ShopObjectType ShopObjectType;
        public Vector2Int CellCoords;
        public string AdditionalData;
    }
}