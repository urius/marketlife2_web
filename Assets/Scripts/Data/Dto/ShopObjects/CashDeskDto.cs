using System;
using UnityEngine;

namespace Data.Dto.ShopObjects
{
    [Serializable]
    public struct CashDeskDto
    {
        public Vector2Int CellCoords;
        public int StaffWorkTimeSecond;
        public int MoneyAmount;

        public CashDeskDto(
            Vector2Int cellCoords,
            int staffWorkTimeSecond,
            int moneyAmount)
        {
            CellCoords = cellCoords;
            StaffWorkTimeSecond = staffWorkTimeSecond;
            MoneyAmount = moneyAmount;
        }
    }
}