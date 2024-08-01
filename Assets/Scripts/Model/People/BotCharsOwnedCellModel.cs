using System.Collections.Generic;
using UnityEngine;

namespace Model.People
{
    public class BotCharsOwnedCellModel
    {
        private readonly Dictionary<Vector2Int, BotCharModelBase> _customerModelByCell = new();
        
        public bool HaveCustomerOnCell(Vector2Int cell)
        {
            return _customerModelByCell.ContainsKey(cell);
        }
        
        public void SetOwnedCell(BotCharModelBase charModel, Vector2Int cellCoords)
        {
            TryRemoveOwnedCell(charModel);

            _customerModelByCell[cellCoords] = charModel;
        }
        
        public bool TryRemoveOwnedCell(BotCharModelBase charModel, Vector2Int cellPosition)
        {
            if (_customerModelByCell.TryGetValue(cellPosition, out var currentOwner)
                && currentOwner == charModel)
            {
                _customerModelByCell.Remove(cellPosition);

                return true;
            }

            return false;
        }
        
        public bool TryRemoveOwnedCell(BotCharModelBase charModel)
        {
            return TryRemoveOwnedCell(charModel, charModel.PreviousCellPosition)
                   || TryRemoveOwnedCell(charModel, charModel.CellCoords);
        }
    }
}