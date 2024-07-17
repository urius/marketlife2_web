using UnityEngine;

namespace Events
{
    public struct SpendMoneyOnBuildPointLastAnimationFinishedEvent
    {
        public readonly Vector2Int TargetBuildPointCellCoords;

        public SpendMoneyOnBuildPointLastAnimationFinishedEvent(Vector2Int targetBuildPointCellCoords)
        {
            TargetBuildPointCellCoords = targetBuildPointCellCoords;
        }
    }
}