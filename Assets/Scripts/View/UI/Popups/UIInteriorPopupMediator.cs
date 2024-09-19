using Data;
using Model.Popups;

namespace View.UI.Popups
{
    public class UIInteriorPopupMediator : MediatorWithModelBase<InteriorPopupViewModel>
    {
        private UITabbedContentPopup _popupView;

        protected override void MediateInternal()
        {
            _popupView = InstantiatePrefab<UITabbedContentPopup>(PrefabKey.UITabbedContentPopup);
        }

        protected override void UnmediateInternal()
        {
            Destroy(_popupView);
            _popupView = null;
        }
    }
}