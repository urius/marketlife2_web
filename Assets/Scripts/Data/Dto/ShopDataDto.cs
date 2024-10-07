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
        [LabeledArray(nameof(CashDeskDto.CellCoords))]
        public CashDeskDto[] CashDesks;
        [LabeledArray(nameof(ShelfDto.CellCoords))]
        public ShelfDto[] Shelfs;
        [LabeledArray(nameof(TruckPointDto.CellCoords))]
        public TruckPointDto[] TruckPoints;
        [LabeledArray(nameof(BuildPointDto.ShopObjectType))]
        public BuildPointDto[] BuildPoints;

        public ShopDataDto(
            Vector2Int size,
            WallType wallType,
            FloorType floorType,
            CashDeskDto[] cashDesks,
            ShelfDto[] shelfs,
            TruckPointDto[] truckPoints,
            BuildPointDto[] buildPoints)
        {
            Size = size;
            WallType = wallType;
            FloorType = floorType;
            CashDesks = cashDesks;
            Shelfs = shelfs;
            TruckPoints = truckPoints;
            BuildPoints = buildPoints;
        }
    }
}