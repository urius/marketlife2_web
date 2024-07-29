using System;
using UnityEngine;

namespace Model.People
{
    public abstract class StaffCharModelBase : ShopCharModelBase
    {
        public event Action<int> WorkSecondsLeftChanged;

        public readonly int WorkSecondsSetting;
        
        public StaffCharModelBase(Vector2Int cellCoords, int workSeconds, int workSecondsLeftSetting) : base(cellCoords)
        {
            WorkSecondsLeft = workSeconds;
            WorkSecondsSetting = workSecondsLeftSetting;
        }
        
        public int WorkSecondsLeft { get; private set; }

        public void AdvanceWorkingTime()
        {
            if (WorkSecondsLeft <= 0) return;
            
            WorkSecondsLeft--;
            WorkSecondsLeftChanged?.Invoke(WorkSecondsLeft);
        }
    }
}