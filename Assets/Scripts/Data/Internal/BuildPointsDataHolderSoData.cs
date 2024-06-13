using System;
using Data.Dto.ShopObjects;
using Other;

namespace Data.Internal
{
    [Serializable]
    public struct ShelfBuildPointRowData
    {
        [LabeledArray(nameof(ShopObjectDto.CellCoords))]
        public BuildPointDto[] Shelfs;
    }
        
    [Serializable]
    public struct TruckGateBuildPointData
    {
        public int BuildCost;
    }
        
    [Serializable]
    public struct TruckGatePositionSettings
    {
        public int FirstGateOffset;
        public int DefaultGateOffset;
    }
}