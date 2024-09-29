using Data.Internal;

namespace Events
{
    public struct UIInteriorPopupTabShownEvent
    {
        public readonly InteriorItemType TabType;

        public UIInteriorPopupTabShownEvent(InteriorItemType tabType)
        {
            TabType = tabType;
        }
    }
}