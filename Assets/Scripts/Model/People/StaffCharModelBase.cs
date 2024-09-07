using System;
using UnityEngine;

namespace Model.People
{
    public abstract class StaffCharModelBase : BotCharModelBase
    {
        public event Action<int> WorkSecondsLeftChanged;

        public StaffCharModelBase(Vector2Int cellCoords, int workSeconds) : base(cellCoords)
        {
            WorkSecondsLeft = workSeconds;
        }
        
        public int WorkSecondsLeft { get; private set; }

        public void AdvanceWorkingTime()
        {
            if (WorkSecondsLeft <= 0) return;
            
            WorkSecondsLeft--;
            WorkSecondsLeftChanged?.Invoke(WorkSecondsLeft);
        }

        public void ProlongWorkTime(int deltaWorkTimeSeconds)
        {
            WorkSecondsLeft += deltaWorkTimeSeconds;
            WorkSecondsLeftChanged?.Invoke(WorkSecondsLeft);
        }
    }
}