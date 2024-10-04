using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model.AdsOffer;

namespace View.UI.LeftPanel.AdsOffer
{
    public abstract class UIAdsOfferMediatorBase<TViewModel> : MediatorWithModelBase<TViewModel>
        where TViewModel : AdsOfferViewModelBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private string _secondsPostfix;
        private UIAdsOfferViewBase _view;

        protected virtual void Subscribe()
        {
            TargetModel.OfferTimeLeftChanged += OnOfferTimeLeftChanged;

            _view.ButtonClick += OnButtonClick;
        }
        
        protected virtual void Unsubscribe()
        {
            TargetModel.OfferTimeLeftChanged -= OnOfferTimeLeftChanged;
            
            _view.ButtonClick -= OnButtonClick;
        }

        private void OnOfferTimeLeftChanged(int timeLeft)
        {
            if (_view == null) return;

            DisplayTimeLeft(_view);
            
            if (timeLeft == 0)
            {
                _view.AnimateDisappearingToLeft();
            }
        }

        protected virtual void SetupView(UIAdsOfferViewBase adsOfferView)
        {
            _secondsPostfix = _localizationProvider.GetLocale(Constants.LocalizationSecondsShortPostfix);

            _view = adsOfferView;

            DisplayTimeLeft(adsOfferView);
        }

        private void DisplayTimeLeft(UIAdsOfferViewBase adsOfferView)
        {
            adsOfferView.SetTimeLeftText(TargetModel.OfferTimeLeft + _secondsPostfix);
            adsOfferView.SetTimeLeftProgress((float)TargetModel.OfferTimeLeft / TargetModel.OfferTimeLeftMax);
        }

        private void OnButtonClick()
        {
            _eventBus.Dispatch(new AdsOfferClickedEvent(TargetModel));
        }
    }
}