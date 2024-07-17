using Model.BuildPoint;

namespace Events
{
    public struct TriggerSpendMoneyOnBuildPointAnimationEvent
    {
        public readonly BuildPointModel BuildPoint;
        public readonly int StartMoneyAmount;
        public readonly int FinishMoneyAmount;

        public TriggerSpendMoneyOnBuildPointAnimationEvent(BuildPointModel buildPoint, int startMoneyAmount, int finishMoneyAmount)
        {
            BuildPoint = buildPoint;
            StartMoneyAmount = startMoneyAmount;
            FinishMoneyAmount = finishMoneyAmount;
        }
    }
}