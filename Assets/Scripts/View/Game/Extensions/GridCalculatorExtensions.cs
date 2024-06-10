using System.Linq;
using UnityEngine;
using Utils;
using View.Game.Shared;

namespace View.Game.Extensions
{
    public static class GridCalculatorExtensions
    {
        public static Vector2Int[] GetOwnedCells(this IGridCalculator gridCalculator, IOwnedCellsView view)
        {
            var ownedCells = view.OwnedCellViews
                .Select(v => gridCalculator.WorldToCell(v.transform.position))
                .ToArray();

            return ownedCells;
        }
    }
}