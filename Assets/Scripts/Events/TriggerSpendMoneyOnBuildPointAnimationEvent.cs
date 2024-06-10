using Model.BuildPoint;

namespace Events
{
    public struct TriggerSpendMoneyOnBuildPointAnimationEvent
    {
        public readonly BuildPointModel BuildPoint;
        public readonly int MoneyAmount;

        public TriggerSpendMoneyOnBuildPointAnimationEvent(BuildPointModel buildPoint, int moneyAmount)
        {
            BuildPoint = buildPoint;
            MoneyAmount = moneyAmount;
        }
    }
}