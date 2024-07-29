using System;
using Model.People.States;
using UnityEngine;

namespace Model.People
{
    public abstract class ShopCharModelBase
    {
        public event Action<ShopSharStateBase> StateChanged;
        public event Action<Vector2Int> CellPositionChanged;

        public bool IsStepInProgress = false;
        
        private Vector2Int _cellCoords;

        protected ShopCharModelBase(Vector2Int cellCoords)
        {
            CellCoords = cellCoords;
        }
        
        
        public ShopSharStateBase State { get; private set; }
        public Vector2Int PreviousCellPosition { get; private set; }

        public Vector2Int CellCoords
        {
            get => _cellCoords;
            private set
            {
                PreviousCellPosition = _cellCoords;
                _cellCoords = value;
                CellPositionChanged?.Invoke(value);
            }
        }

        public void SetCellPosition(Vector2Int stepCell)
        {
            CellCoords = stepCell;
        }
        
        protected void SetState(ShopSharStateBase state)
        {
            State = state;

            StateChanged?.Invoke(State);
        }
    }
}