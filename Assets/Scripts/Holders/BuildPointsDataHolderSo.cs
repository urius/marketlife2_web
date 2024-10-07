using Data;
using Data.Dto.ShopObjects;
using Data.Internal;
using Infra.Instance;
using Other;
using UnityEngine;
using UnityEngine.Assertions;

namespace Holders
{
    [CreateAssetMenu(fileName = "BuildPointsDataHolderSo", menuName = "ScriptableObjects/BuildPointsDataHolderSo")]
    public class BuildPointsDataHolderSo : ScriptableObject
    {
        [LabeledArray(nameof(CashDeskBuildPointRowData.CellCoords))]
        [SerializeField]
        private CashDeskBuildPointRowData[] _cashDesksData;
        
        [SerializeField]
        private ShelfBuildPointRowData[] _shelfsByRow;

        [SerializeField] 
        private TruckGatePositionSettings _truckGatePositionSettings;
        
        [SerializeField]
        private TruckGateBuildPointData[] _truckGates;

        public BuildPointDto GetCashDeskBuildPointData(int index)
        {
            if (index < _cashDesksData.Length)
            {
                return ToBuildPointDto(_cashDesksData[index]);
            }
            else
            {
                var buildCost = InterpolateBuildCostFor(index, _cashDesksData.Length - 1,
                    _cashDesksData[^1].Cost,
                    _cashDesksData[^2].Cost);
                var coords = InterpolateCellCoordsFor(index, _cashDesksData.Length - 1,
                    _cashDesksData[^1].CellCoords,
                    _cashDesksData[^2].CellCoords);

                return new BuildPointDto(BuildPointType.BuildShopObject, ShopObjectType.CashDesk, coords, buildCost);
            }
        }

        public bool TryGetShelfBuildPointData(int rowIndex, int index, out BuildPointDto buildPointDto)
        {
            var xCoordDefault = Interpolate(_shelfsByRow[0].Shelfs[0].XCellCoord, _shelfsByRow[0].Shelfs[1].XCellCoord,
                index);
            var yCoordDefault = Interpolate(_shelfsByRow[0].YCellCoord, _shelfsByRow[1].YCellCoord, rowIndex);
            
            if (rowIndex < _shelfsByRow.Length)
            {
                var shelfsInRow = _shelfsByRow[rowIndex].Shelfs;
                var yCoord = _shelfsByRow[rowIndex].YCellCoord;
                yCoord = yCoord <= 0 ? yCoordDefault : yCoord;
                
                if (index < shelfsInRow.Length)
                {
                    buildPointDto = ToBuildPointDto(yCoord, shelfsInRow[index], xCoordDefault);
                }
                else
                {
                    var buildCost = InterpolateBuildCostFor(index, shelfsInRow.Length - 1,
                        shelfsInRow[^1].Cost,
                        shelfsInRow[^2].Cost);
                    
                    var coords = shelfsInRow[^1].XCellCoord <= 0
                        ? new Vector2Int(xCoordDefault, yCoord)
                        : InterpolateCellCoordsFor(index, shelfsInRow.Length - 1,
                            new Vector2Int(shelfsInRow[^1].XCellCoord, yCoord),
                            new Vector2Int(shelfsInRow[^2].XCellCoord, yCoord));

                    buildPointDto = new BuildPointDto(
                        BuildPointType.BuildShopObject, shelfsInRow[0].ShopObjectType, coords, buildCost);
                }

                return true;
            }

            buildPointDto = default;
            
            return false;
        }

        private static int Interpolate(int firstValue, int secondValue, int targetIndex)
        {
            return firstValue + (secondValue - firstValue) * targetIndex;
        }

        public int RowIndexToYCoord(int rowIndex)
        {
            var firstRowYCoord = _shelfsByRow[0].YCellCoord;
            var secondRowYCoords = _shelfsByRow[1].YCellCoord;

            return firstRowYCoord + rowIndex * (secondRowYCoords - firstRowYCoord);
        }

        public int YCoordToRowIndex(int yCoord)
        {
            var firstRowYCoord = _shelfsByRow[0].YCellCoord;
            var secondRowYCoords = _shelfsByRow[1].YCellCoord;
            var deltaY = secondRowYCoords - firstRowYCoord;

            var rowIndex = 0;
            
            while (rowIndex < 100)
            {
                if (firstRowYCoord + rowIndex * deltaY == yCoord)
                {
                    return rowIndex;
                }

                rowIndex++;
            }

            return -1;
        }

        public bool TryGetTruckGateBuildPointData(int truckGateIndex, out BuildPointDto result)
        {
            result = default;

            var truckPointSettingsProvider = Instance.Get<TruckPointsSettingsProviderSo>();

            var haveSettings = truckPointSettingsProvider.HaveSettings(truckGateIndex);
            
            if (haveSettings)
            {
                var cellCoords = GetTruckPointCoordsByIndex(truckGateIndex);
                var buildCost = GetTruckGatesBuildCost(truckGateIndex);

                result = new BuildPointDto(
                    BuildPointType.BuildShopObject, ShopObjectType.TruckPoint, cellCoords, buildCost);
            }

            return haveSettings;
        }

        private int GetTruckGatesBuildCost(int truckGateIndex)
        {
            if (truckGateIndex < _truckGates.Length)
            {
                return _truckGates[truckGateIndex].BuildCost;
            }
            else
            {
                return InterpolateBuildCostFor(truckGateIndex, _truckGates.Length - 1,
                    _truckGates[^1].BuildCost,
                    _truckGates[^2].BuildCost);
            }
        }

        public Vector2Int GetTruckPointCoordsByIndex(int truckPointIndex)
        {
            return new Vector2Int(0,
                _truckGatePositionSettings.FirstGateOffset +
                truckPointIndex * _truckGatePositionSettings.DefaultGateOffset);
        }

        public int GetTruckPointIndexByCoords(Vector2Int truckPointCoords)
        {
            var relativeYCoord = truckPointCoords.y - _truckGatePositionSettings.FirstGateOffset;
            if (relativeYCoord % _truckGatePositionSettings.DefaultGateOffset == 0)
            {
                return relativeYCoord / _truckGatePositionSettings.DefaultGateOffset;
            }

            return -1;
        }

        private static BuildPointDto ToBuildPointDto(CashDeskBuildPointRowData cashDeskBuildPointRowData)
        {
            return new BuildPointDto(BuildPointType.BuildShopObject, ShopObjectType.CashDesk,
                cashDeskBuildPointRowData.CellCoords, cashDeskBuildPointRowData.Cost);
        }

        private static int InterpolateBuildCostFor(int index, int lastIndex, int lastItemCost, int preLastItemCost)
        {
            var deltaMoney = lastItemCost - preLastItemCost;
            var deltaIndex = index - lastIndex;

            Assert.IsTrue(deltaIndex > 0);
            Assert.IsTrue(deltaMoney >= 0);

            return deltaIndex * deltaMoney + lastItemCost;
        }

        private static Vector2Int InterpolateCellCoordsFor(int index, int lastIndex, Vector2Int lastItemCellCoords, Vector2Int preLastItemCellCoords)
        {
            var deltaIndex = index - lastIndex;
            var deltaCoords = lastItemCellCoords - preLastItemCellCoords;
            
            Assert.IsTrue(deltaIndex > 0);

            return deltaIndex * deltaCoords + lastItemCellCoords;
        }

        private static BuildPointDto ToBuildPointDto(int yCoord, ShelfBuildPointData shelfBuildPointData,
            int xCoordDefault)
        {
            var xCoord = shelfBuildPointData.XCellCoord <= 0 ? xCoordDefault : shelfBuildPointData.XCellCoord;
            
            return new BuildPointDto(
                BuildPointType.BuildShopObject,
                shelfBuildPointData.ShopObjectType,
                new Vector2Int(xCoord, yCoord),
                shelfBuildPointData.Cost);
        }
    }
}