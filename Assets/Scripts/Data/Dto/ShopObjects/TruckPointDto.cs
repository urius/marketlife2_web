using System;
using UnityEngine;

namespace Data.Dto.ShopObjects
{
    [Serializable]
    public struct TruckPointDto
    {
        public Vector2Int CellCoords;
        public int[] ProductBoxes;
        public int UpgradesCount;
        public int DeliverTimeSecondsRest;
        public TruckPointStaffCharDto StaffData;

        public TruckPointDto(
            Vector2Int cellCoords,
            int[] productBoxes,
            int upgradesCount,
            int deliverTimeSecondsRest,
            TruckPointStaffCharDto staffData)
        {
            CellCoords = cellCoords;
            ProductBoxes = productBoxes;
            UpgradesCount = upgradesCount;
            DeliverTimeSecondsRest = deliverTimeSecondsRest;
            StaffData = staffData;
        }
    }
}