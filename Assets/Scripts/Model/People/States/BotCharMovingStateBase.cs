using UnityEngine;

namespace Model.People.States
{
    public abstract class BotCharMovingStateBase : BotCharStateBase
    {
        public readonly Vector2Int TargetCell;

        public BotCharMovingStateBase(Vector2Int targetCell)
        {
            TargetCell = targetCell;
        }
    }
}