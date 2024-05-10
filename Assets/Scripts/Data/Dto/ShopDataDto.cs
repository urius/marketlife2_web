using System;
using Data.Dto.ShopObjects;
using Other;
using UnityEngine;

namespace Data.Dto
{
    [Serializable]
    public struct ShopDataDto
    {
        public Vector2Int Size;
        public WallType WallType;
        public FloorType FloorType;
        [LabeledArray(nameof(ShopObjectDto.ShopObjectType))]
        public ShopObjectDto[] ShopObjects;
        [LabeledArray(nameof(ShopObjectDto.ShopObjectType))]
        public BuildPointDto[] BuildPoints;
    }
}