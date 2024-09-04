using Model.SpendPoints;

namespace Events
{
    public struct ExpandPointUnlockedEvent
    {
        public readonly BuildPointModel BuildPointModel;

        public ExpandPointUnlockedEvent(BuildPointModel buildPointModel)
        {
            BuildPointModel = buildPointModel;
        }
    }
}