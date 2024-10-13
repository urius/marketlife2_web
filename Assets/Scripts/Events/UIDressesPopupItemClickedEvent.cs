using Model.Popups;

namespace Events
{
    public struct UIDressesPopupItemClickedEvent
    {
        public readonly DressesPopupItemViewModel ItemViewModel;

        public UIDressesPopupItemClickedEvent(DressesPopupItemViewModel itemViewModel)
        {
            ItemViewModel = itemViewModel;
        }
    }
}