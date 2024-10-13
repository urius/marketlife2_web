using System;
using UnityEngine;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerCharDataDto
    {
        public Vector2Int CellPosition;
        public int[] ProductsInBox;
        public PlayerDressesDto DressesDto;

        public PlayerCharDataDto(
            Vector2Int cellPosition,
            int[] productsInBox,
            PlayerDressesDto dressesDto)
        {
            CellPosition = cellPosition;
            ProductsInBox = productsInBox;
            DressesDto = dressesDto;
        }
    }
}