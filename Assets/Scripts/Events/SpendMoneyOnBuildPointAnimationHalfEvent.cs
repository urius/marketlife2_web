using UnityEngine;

namespace Events
{
    public struct SpendMoneyOnBuildPointAnimationHalfEvent
    {
        public readonly Vector2Int TargetBuildPointCellCoords;
        public readonly int ActiveAnimationsAmount;

        public SpendMoneyOnBuildPointAnimationHalfEvent(Vector2Int targetBuildPointCellCoords, int activeAnimationsAmount)
        {
            TargetBuildPointCellCoords = targetBuildPointCellCoords;
            ActiveAnimationsAmount = activeAnimationsAmount;
        }
    }
}