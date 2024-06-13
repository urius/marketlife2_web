using System;
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
                var buildCost = InterpolateBuildCostFor(index, _cashDesks.Length - 1, _cashDesks[^1], _cashDesks[^2]);
                var coords = InterpolateCellCoordsFor(index, _cashDesks.Length - 1, _cashDesks[^1], _cashDesks[^2]);
                
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
                
                if (index < shelfsInRow.Length)
                {
                    buildPointDto = shelfsInRow[index];
                }
                else
                {
                    var buildCost = InterpolateBuildCostFor(index, shelfsInRow.Length - 1, shelfsInRow[^1], shelfsInRow[^2]);
                    var coords = InterpolateCellCoordsFor(index, shelfsInRow.Length - 1, shelfsInRow[^1], shelfsInRow[^2]);
                
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
                    CellCoords = new Vector2Int(0, _truckGatePositionSettings.FirstGateOffset + truckGateIndex * _truckGatePositionSettings.DefaultGateOffset),
                    MoneyToBuildLeft = data.BuildCost,
                };
            }

            return haveData;
        }

        private static int InterpolateBuildCostFor(int index, int lastIndex, BuildPointDto lastItem,
            BuildPointDto preLastItem)
        {
            var deltaMoney = lastItem.MoneyToBuildLeft - preLastItem.MoneyToBuildLeft;
            var deltaIndex = index - lastIndex;

            Assert.IsTrue(deltaIndex > 0);
            Assert.IsTrue(deltaMoney >= 0);

            return deltaIndex * deltaMoney + lastItem.MoneyToBuildLeft;
        }

        private static Vector2Int InterpolateCellCoordsFor(int index, int lastIndex, BuildPointDto lastItem, BuildPointDto preLastItem)
        {
            var deltaIndex = index - lastIndex;
            var deltaCoords = lastItem.CellCoords - preLastItem.CellCoords;
            
            Assert.IsTrue(deltaIndex > 0);

            return deltaIndex * deltaCoords + lastItem.CellCoords;
        }
    }
}