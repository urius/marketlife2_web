using System;
using Data;
using Holders;
using Infra.Instance;
using Model;
using Model.People;
using UnityEngine;

namespace Systems
{
    public abstract class BotCharsControlSystemBase : ISystem
    {
        
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        
        private ShopModel _shopModel;
        private CustomersModel _customersModel;

        protected abstract void BeforeChangeCell(BotCharModelBase charModel, Vector2Int stepCell);
        
        public virtual void Start()
        {
            _shopModel = _shopModelHolder.ShopModel;
            _customersModel = _shopModel.CustomersModel;
        }

        public virtual void Stop()
        {
            
        }
        
        protected void MakeNextStep(BotCharModelBase charModel, Vector2Int targetPoint)
        {
            var stepCell = Vector2Int.zero;
            var minDistanceToTarget = -1f;
            
            foreach (var nearCellOffset in Constants.NearCells4)
            {
                var nearCell = charModel.CellCoords + nearCellOffset;
                
                if (nearCell == charModel.PreviousCellPosition 
                    || nearCell == charModel.Previous3CellPosition
                    || CanMakeStepTo(nearCell) == false)
                {
                    continue;
                }

                var distanceToTarget = (targetPoint - nearCell).magnitude;
                
                if (minDistanceToTarget < 0 || distanceToTarget < minDistanceToTarget)
                {
                    minDistanceToTarget = distanceToTarget;
                    stepCell = nearCell;
                }
            }

            if (minDistanceToTarget < 0)
            {
                if (CanMakeStepTo(charModel.PreviousCellPosition))
                {
                    stepCell = charModel.PreviousCellPosition;
                    minDistanceToTarget = (targetPoint - stepCell).magnitude;
                }
                else if (IsOneStepAway(charModel.CellCoords, charModel.Previous3CellPosition) 
                         && CanMakeStepTo(charModel.Previous3CellPosition))
                {
                    stepCell = charModel.Previous3CellPosition;
                    minDistanceToTarget = (targetPoint - stepCell).magnitude;
                }
            }

            if (minDistanceToTarget >= 0)
            {
                BeforeChangeCell(charModel, stepCell);
            
                charModel.SetCellPosition(stepCell);
                charModel.IsStepInProgress = true;
            }
            else
            {
                Debug.LogWarning("No cell to move for char");
            }
        }

        protected virtual bool CanMakeStepTo(Vector2Int cell)
        {
            return IsWalkable(cell);
        }

        private static bool IsOneStepAway(Vector2Int coordsA, Vector2Int coordsB)
        {
            return coordsA.x == coordsB.x && Math.Abs(coordsA.y - coordsB.y) == 1
                   || coordsA.y == coordsB.y && Math.Abs(coordsA.x - coordsB.x) == 1;
        }

        private bool IsWalkable(Vector2Int cell)
        {
            if (cell.x < 0 || cell.x >= _shopModel.Size.x) return false;
            if (cell.y < Constants.YTopWalkableCoordForCustomers || cell.y >= _shopModel.Size.y) return false;
            if (cell.y == -1) return _shopModel.HaveDoorOn(cell.x);
            
            return _ownedCellsDataHolder.IsWalkableForBotChar(cell);
        }
    }
}