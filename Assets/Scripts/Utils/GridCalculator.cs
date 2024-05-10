using UnityEngine;

namespace Utils
{
    public class GridCalculator : IGridCalculator
    {
        private readonly Grid _grid;

        public GridCalculator(Grid grid)
        {
            _grid = grid;
            WorldCellXDirection = (GetCellCenterWorld(new Vector2Int(1,0)) - GetCellCenterWorld(Vector2Int.zero)).normalized;
            WorldCellYDirection = (GetCellCenterWorld(new Vector2Int(0,1)) - GetCellCenterWorld(Vector2Int.zero)).normalized;
        }

        public Vector2 CellSize => _grid.cellSize;
        public Vector3 WorldCellXDirection { get; }
        public Vector3 WorldCellYDirection { get; }

        public Vector3 CellToWorld(Vector2Int cellPosition)
        {
            return _grid.CellToWorld((Vector3Int)cellPosition);
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            return (Vector2Int)_grid.WorldToCell(worldPosition);
        }

        public Vector3 GetCellCenterWorld(Vector2Int cellPosition)
        {
            return _grid.GetCellCenterWorld((Vector3Int)cellPosition);
        }
    }

    public interface IGridCalculator
    {
        public Vector2 CellSize { get; }
        public Vector3 WorldCellXDirection { get; }
        public Vector3 WorldCellYDirection { get; }
        
        public Vector3 CellToWorld(Vector2Int cellPosition);
        public Vector2Int WorldToCell(Vector3 worldPosition);
        public Vector3 GetCellCenterWorld(Vector2Int cellPosition);
    }
}