using System;
using UnityEngine;

namespace Model
{
    public class PlayerCharModel
    {
        public event Action<Vector2Int> CellPositionChanged;
        
        public PlayerCharModel(Vector2Int cellPosition)
        {
            CellPosition = cellPosition;
        }

        public Vector2Int CellPosition { get; private set; }

        public void SetCellPosition(Vector2Int cellPosition)
        {
            if (CellPosition == cellPosition) return;
            
            CellPosition = cellPosition;
            CellPositionChanged?.Invoke(CellPosition);
        }
    }
}