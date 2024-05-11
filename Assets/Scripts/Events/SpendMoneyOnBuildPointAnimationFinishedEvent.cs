using UnityEngine;

namespace Events
{
    public struct SpendMoneyOnBuildPointAnimationFinishedEvent
    {
        public readonly Vector2Int TargetBuildPointCellCoords;

        public SpendMoneyOnBuildPointAnimationFinishedEvent(Vector2Int targetBuildPointCellCoords)
        {
            TargetBuildPointCellCoords = targetBuildPointCellCoords;
        }
    }
}