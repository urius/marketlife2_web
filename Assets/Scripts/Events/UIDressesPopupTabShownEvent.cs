using Model.Popups;

namespace Events
{
    public struct UIDressesPopupTabShownEvent
    {
        public readonly PlayerDressesPopupTabType TabType;

        public UIDressesPopupTabShownEvent(PlayerDressesPopupTabType tabType)
        {
            TabType = tabType;
        }
    }
}