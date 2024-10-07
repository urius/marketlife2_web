using System;
using UnityEngine;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerCharDataDto
    {
        public Vector2Int CellPosition;
        public int[] ProductsInBox;

        public PlayerCharDataDto(Vector2Int cellPosition, int[] productsInBox)
        {
            CellPosition = cellPosition;
            ProductsInBox = productsInBox;
        }
    }
}