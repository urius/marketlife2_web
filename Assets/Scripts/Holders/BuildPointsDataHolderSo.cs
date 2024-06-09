using System;
using Data;
using Data.Dto.ShopObjects;
using Other;
using UnityEngine;
using UnityEngine.Assertions;

namespace Holders
{
    [CreateAssetMenu(fileName = "BuildPointsDataHolderSo", menuName = "ScriptableObjects/BuildPointsDataHolderSo")]
    public class BuildPointsDataHolderSo : ScriptableObject
    {
        [LabeledArray(nameof(ShopObjectDto.CellCoords))]
        public BuildPointDto[] CashDesks;
        
        public ShelfBuildPointRowData[] ShelfsByRow;
        
        [LabeledArray(nameof(ShopObjectDto.CellCoords))]
        public BuildPointDto[] TruckGates;

        [Serializable]
        public struct ShelfBuildPointRowData
        {
            [LabeledArray(nameof(ShopObjectDto.CellCoords))]
            public BuildPointDto[] Shelfs;
        }

        public BuildPointDto GetCashDeskBuildPointData(int index)
        {
            if (index < CashDesks.Length)
            {
                return CashDesks[index];
            }
            else
            {
                var buildCost = InterpolateBuildCostFor(index, CashDesks.Length - 1, CashDesks[^1], CashDesks[^2]);
                var coords = InterpolateCellCoordsFor(index, CashDesks.Length - 1, CashDesks[^1], CashDesks[^2]);
                
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
            if (rowIndex < ShelfsByRow.Length)
            {
                var shelfsInRow = ShelfsByRow[rowIndex].Shelfs;
                
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