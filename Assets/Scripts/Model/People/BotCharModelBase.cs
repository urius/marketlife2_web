using System;
using Model.People.States;
using UnityEngine;

namespace Model.People
{
    public abstract class BotCharModelBase
    {
        public event Action<BotCharStateBase> StateChanged;
        public event Action<Vector2Int> CellPositionChanged;

        public bool IsStepInProgress = false;
        private Vector2Int _cellCoords;

        protected BotCharModelBase(Vector2Int cellCoords)
        {
            CellCoords = cellCoords;
        }
        
        public BotCharStateBase State { get; private set; }
        public Vector2Int PreviousCellPosition { get; private set; }
        public Vector2Int Previous2CellPosition { get; private set; }
        public Vector2Int Previous3CellPosition { get; private set; }

        public Vector2Int CellCoords
        {
            get => _cellCoords;
            private set
            {
                Previous3CellPosition = Previous2CellPosition;
                Previous2CellPosition = PreviousCellPosition;
                PreviousCellPosition = _cellCoords;
                _cellCoords = value;
                CellPositionChanged?.Invoke(value);
            }
        }

        public void SetCellPosition(Vector2Int stepCell)
        {
            CellCoords = stepCell;
        }
        
        protected void SetState(BotCharStateBase state)
        {
            State = state;

            StateChanged?.Invoke(State);
        }
    }
}