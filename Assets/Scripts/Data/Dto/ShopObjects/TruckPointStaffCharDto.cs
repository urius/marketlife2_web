using System;
using UnityEngine;

namespace Data.Dto.ShopObjects
{
    [Serializable]
    public struct TruckPointStaffCharDto
    {
        public Vector2Int CellCoords;
        public int WorkSecondsLeft;
        public int[] Products;

        public TruckPointStaffCharDto(
            Vector2Int cellCoords,
            int workSecondsLeft,
            int[] products)
        {
            CellCoords = cellCoords;
            WorkSecondsLeft = workSecondsLeft;
            Products = products;
        }
    }
}