using Model.Popups;

namespace Events
{
    public struct UIInteriorPopupItemClickedEvent
    {
        public readonly InteriorPopupItemViewModelBase ItemViewModel;

        public UIInteriorPopupItemClickedEvent(InteriorPopupItemViewModelBase itemViewModel)
        {
            ItemViewModel = itemViewModel;
        }
    }
}