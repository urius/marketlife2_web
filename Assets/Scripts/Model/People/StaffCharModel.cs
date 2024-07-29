using System;
using UnityEngine;

namespace Model.People
{
    public class StaffCharModel : ShopCharModelBase
    {
        public event Action<int> WorkSecondsLeftChanged;

        public readonly int WorkSecondsSetting;
        
        public StaffCharModel(Vector2Int cellCoords, int workSeconds) : base(cellCoords)
        {
            WorkSecondsLeft = workSeconds;
            WorkSecondsSetting = WorkSecondsLeft;
            CellCoords = cellCoords;
        }
        
        public int WorkSecondsLeft { get; private set; }
        public Vector2Int CellCoords { get; private set; }

        public void AdvanceWorkingTime()
        {
            if (WorkSecondsLeft <= 0) return;
            
            WorkSecondsLeft--;
            WorkSecondsLeftChanged?.Invoke(WorkSecondsLeft);
        }
    }
}