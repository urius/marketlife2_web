using System;
using Data.Dto.ShopObjects;
using Other;

namespace Data.Internal
{
    [Serializable]
    public struct ShelfBuildPointRowData
    {
        public int YCellCoord;
        
        [LabeledArray(nameof(ShopObjectDto.CellCoords))]
        public ShelfBuildPointData[] Shelfs;
    }
    
    [Serializable]
    public struct ShelfBuildPointData
    {
        public ShopObjectType ShopObjectType;
        public int XCellCoord;
        public int Cost;
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