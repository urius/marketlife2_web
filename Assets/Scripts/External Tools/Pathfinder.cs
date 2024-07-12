using System.Collections.Generic;

namespace Tools
{
    public class Pathfinder
    {
        public static TCell[] FindPath<TCell>(ICellsProvider<TCell> cellsProvider, TCell start, TCell end)
        {
            var openList = new Queue<TCell>(new[] { start });
            var cellsInfo = new Dictionary<TCell, CellInfo<TCell>>();
            cellsInfo[start] = new CellInfo<TCell>(start, start, 0);

            while (openList.Count > 0)
            {
                var currentCellInfo = cellsInfo[openList.Dequeue()];

                var nearCells = cellsProvider.GetWalkableNearCells(currentCellInfo.Current);
                foreach (var cell in nearCells)
                {
                    if (!cellsInfo.ContainsKey(cell))
                    {
                        cellsInfo[cell] = new CellInfo<TCell>(cell, currentCellInfo.Current, currentCellInfo.TotalMoveCost + cellsProvider.GetCellMoveCost(cell));
                        openList.Enqueue(cell);
                    }
                    else
                    {
                        var cellInfo = cellsInfo[cell];
                        var totalMoveCost = currentCellInfo.TotalMoveCost + cellsProvider.GetCellMoveCost(cell);
                        if (totalMoveCost < cellInfo.TotalMoveCost)
                        {
                            cellInfo.Back = currentCellInfo.Current;
                            cellInfo.TotalMoveCost = totalMoveCost;
                        }
                    }
                }
            }

            var result = new List<TCell>() { end };
            if (cellsInfo.TryGetValue(end, out var tempCellInfo))
            {

                while (!cellsProvider.IsCellEquals(tempCellInfo.Back, tempCellInfo.Current))
                {
                    result.Add(tempCellInfo.Back);
                    tempCellInfo = cellsInfo[tempCellInfo.Back];
                }
                result.Reverse();
                return result.ToArray();
            }
            else
            {
                return new TCell[0];
            }
        }

        private class CellInfo<TCell>
        {
            public CellInfo(TCell current, TCell back, int totalMoveCost)
            {
                Current = current;
                Back = back;
                TotalMoveCost = totalMoveCost;
            }

            public TCell Current;
            public TCell Back;
            public int TotalMoveCost;
        }
    }


    public interface ICellsProvider<TCell>
    {
        IEnumerable<TCell> GetWalkableNearCells(TCell cell);
        int GetCellMoveCost(TCell cell);
        bool IsCellEquals(TCell cellA, TCell cellB);
    }
}