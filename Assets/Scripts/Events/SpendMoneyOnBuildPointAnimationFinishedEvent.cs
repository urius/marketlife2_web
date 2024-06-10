using UnityEngine;

namespace Events
{
    public struct SpendMoneyOnBuildPointAnimationFinishedEvent
    {
        public readonly Vector2Int TargetBuildPointCellCoords;
        public readonly int MoneyAmount;

        public SpendMoneyOnBuildPointAnimationFinishedEvent(Vector2Int targetBuildPointCellCoords,
            int moneyAmount)
        {
            TargetBuildPointCellCoords = targetBuildPointCellCoords;
            MoneyAmount = moneyAmount;
        }
    }
}