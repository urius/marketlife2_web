using Model.Popups;

namespace Events
{
    public struct UIClosePopupClickedEvent
    {
        public readonly PopupViewModelBase PopupViewModel;

        public UIClosePopupClickedEvent(PopupViewModelBase popupViewModel)
        {
            PopupViewModel = popupViewModel;
        }
    }
}