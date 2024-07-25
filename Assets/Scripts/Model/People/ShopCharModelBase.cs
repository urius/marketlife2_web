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
        
        private Vector2Int _cellPosition;

        protected ShopCharModelBase(Vector2Int cellPosition)
        {
            CellPosition = cellPosition;
        }
        
        
        public ShopSharStateBase State { get; private set; }
        public Vector2Int PreviousCellPosition { get; private set; }

        public Vector2Int CellPosition
        {
            get => _cellPosition;
            private set
            {
                PreviousCellPosition = _cellPosition;
                _cellPosition = value;
                CellPositionChanged?.Invoke(value);
            }
        }

        public void SetCellPosition(Vector2Int stepCell)
        {
            CellPosition = stepCell;
        }
        
        protected void SetState(ShopSharStateBase state)
        {
            State = state;

            StateChanged?.Invoke(State);
        }
    }
}