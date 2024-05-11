using Model.BuildPoint;

namespace Events
{
    public struct TriggerSpendMoneyOnBuildPointAnimationEvent
    {
        public readonly BuildPointModel BuildPoint;

        public TriggerSpendMoneyOnBuildPointAnimationEvent(BuildPointModel buildPoint)
        {
            BuildPoint = buildPoint;
        }
    }
}