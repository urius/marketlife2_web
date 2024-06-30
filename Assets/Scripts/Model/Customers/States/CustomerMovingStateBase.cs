using UnityEngine;

namespace Model.Customers.States
{
    public abstract class CustomerMovingStateBase : CustomerStateBase
    {
        public readonly Vector2Int TargetCell;

        public CustomerMovingStateBase(Vector2Int targetCell)
        {
            TargetCell = targetCell;
        }
    }
}