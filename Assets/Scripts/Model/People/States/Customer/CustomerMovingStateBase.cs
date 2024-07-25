using UnityEngine;

namespace Model.People.States.Customer
{
    public abstract class CustomerMovingStateBase : ShopSharStateBase
    {
        public readonly Vector2Int TargetCell;

        public CustomerMovingStateBase(Vector2Int targetCell)
        {
            TargetCell = targetCell;
        }
    }
}