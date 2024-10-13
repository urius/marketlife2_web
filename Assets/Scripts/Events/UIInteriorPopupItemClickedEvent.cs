using Model.Popups;

namespace Events
{
    public struct UIInteriorPopupItemClickedEvent
    {
        public readonly PopupItemViewModelBase ItemViewModel;

        public UIInteriorPopupItemClickedEvent(PopupItemViewModelBase itemViewModel)
        {
            ItemViewModel = itemViewModel;
        }
    }
}