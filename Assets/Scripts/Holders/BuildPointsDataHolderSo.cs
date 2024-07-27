using Data;
using Data.Dto.ShopObjects;
using Data.Internal;
using Other;
using UnityEngine;
using UnityEngine.Assertions;

namespace Holders
{
    [CreateAssetMenu(fileName = "BuildPointsDataHolderSo", menuName = "ScriptableObjects/BuildPointsDataHolderSo")]
    public class BuildPointsDataHolderSo : ScriptableObject
    {
        [LabeledArray(nameof(ShopObjectDto.CellCoords))]
        [SerializeField]
        private BuildPointDto[] _cashDesks;
        
        [SerializeField]
        private ShelfBuildPointRowData[] _shelfsByRow;

        [SerializeField] 
        private TruckGatePositionSettings _truckGatePositionSettings;
        
        [SerializeField]
        private TruckGateBuildPointData[] _truckGates;

        public BuildPointDto GetCashDeskBuildPointData(int index)
        {
            if (index < _cashDesks.Length)
            {
                return _cashDesks[index];
            }
            else
            {
                var buildCost = InterpolateBuildCostFor(index, _cashDesks.Length - 1,
                    _cashDesks[^1].MoneyToBuildLeft,
                    _cashDesks[^2].MoneyToBuildLeft);
                var coords = InterpolateCellCoordsFor(index, _cashDesks.Length - 1,
                    _cashDesks[^1].CellCoords,
                    _cashDesks[^2].CellCoords);
                
                return new BuildPointDto()
                {
                    ShopObjectType = ShopObjectType.CashDesk,
                    MoneyToBuildLeft = buildCost,
                    CellCoords = coords,
                };
            }
        }

        public bool TryGetShelfBuildPointData(int rowIndex, int index, out BuildPointDto buildPointDto)
        {
            if (rowIndex < _shelfsByRow.Length)
            {
                var shelfsInRow = _shelfsByRow[rowIndex].Shelfs;
                var yCoord = _shelfsByRow[rowIndex].YCellCoord;
                
                if (index < shelfsInRow.Length)
                {
                    buildPointDto = ToBuildPointDto(yCoord, shelfsInRow[index]);
                }
                else
                {
                    var buildCost = InterpolateBuildCostFor(index, shelfsInRow.Length - 1,
                        shelfsInRow[^1].Cost,
                        shelfsInRow[^2].Cost);
                    var coords = InterpolateCellCoordsFor(index, shelfsInRow.Length - 1,
                        new Vector2Int(shelfsInRow[^1].XCellCoord, yCoord),
                        new Vector2Int(shelfsInRow[^2].XCellCoord, yCoord));
                
                    buildPointDto = new BuildPointDto()
                    {
                        ShopObjectType = shelfsInRow[0].ShopObjectType,
                        MoneyToBuildLeft = buildCost,
                        CellCoords = coords,
                    };
                }

                return true;
            }

            buildPointDto = default;
            
            return false;
        }

        public int RowIndexToYCoord(int rowIndex)
        {
            var firstRowYCoord = _shelfsByRow[0].YCellCoord;
            var secondRowYCoords = _shelfsByRow[1].YCellCoord;

            return firstRowYCoord + rowIndex * (secondRowYCoords - firstRowYCoord);
        }

        public bool TryGetTruckGateBuildPointData(int truckGateIndex, out BuildPointDto result)
        {
            result = default;
            
            var haveData = truckGateIndex < _truckGates.Length;
            
            if (haveData)
            {
                var data = _truckGates[truckGateIndex];
                
                result = new BuildPointDto()
                {
                    ShopObjectType = ShopObjectType.TruckPoint,
                    CellCoords = GetTruckPointCoordsByIndex(truckGateIndex),
                    MoneyToBuildLeft = data.BuildCost,
                };
            }

            return haveData;
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

        private static BuildPointDto ToBuildPointDto(int yCoord, ShelfBuildPointData shelfBuildPointData)
        {
            return new BuildPointDto()
            {
                ShopObjectType = shelfBuildPointData.ShopObjectType,
                CellCoords = new Vector2Int(shelfBuildPointData.XCellCoord, yCoord),
                MoneyToBuildLeft = shelfBuildPointData.Cost,
            };
        }
    }
}