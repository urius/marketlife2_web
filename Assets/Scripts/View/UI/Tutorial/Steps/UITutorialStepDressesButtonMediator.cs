using Data;
using Holders;
using Infra.Instance;
using Model.Popups;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepDressesButtonMediator : UITutorialStepMediatorBase
    {
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly ISharedFlagsHolder _sharedFlagsHolder = Instance.Get<ISharedFlagsHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IPopupViewModelsHolder _popupViewModelsHolder = Instance.Get<IPopupViewModelsHolder>();
        
        private UITutorialStepUIPointerView _view;

        protected override void MediateInternal()
        {
        }

        protected override bool CheckStepConditions()
        {
            var dressesButtonShownFlag = _sharedFlagsHolder.Get(SharedFlagKey.UITopPanelDressesButtonShown);
            
            return dressesButtonShownFlag;
        }

        protected override void ActivateStep()
        {
            _view = InstantiateColdPrefab<UITutorialStepUIPointerView>(Constants.TutorialStepPointUIPath);
            
            _view.ToTopLeftSideState();

            var dressesButtonTransform = _sharedViewsDataHolder.GetTopPanelDressesButtonTransform();
            
            _view.SetText(_localizationProvider.GetLocale(Constants.LocalizationTutorialDressesButtonMessageKey));
            
            _view.SetPointerToPosition(dressesButtonTransform.position, new Vector2(0, -10));

            SubscribeOnActivate();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();

            if (_view != null)
            {
                Destroy(_view);
                _view = null;
            }
        }

        private void SubscribeOnActivate()
        {
            _popupViewModelsHolder.PopupAdded += OnPopupAdded;
        }

        private void Unsubscribe()
        {
            _popupViewModelsHolder.PopupAdded -= OnPopupAdded;
        }

        private void OnPopupAdded(PopupViewModelBase popupViewModel)
        {
            if (popupViewModel.PopupKey == PopupKey.PlayerDressesPopup)
            {
                DispatchStepFinished();
            }
        }
    }
}