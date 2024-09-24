using Model.Popups;

namespace Events
{
    public struct UIRequestClosePopupEvent
    {
        public readonly PopupViewModelBase PopupViewModel;

        public UIRequestClosePopupEvent(PopupViewModelBase popupViewModel)
        {
            PopupViewModel = popupViewModel;
        }
    }
}