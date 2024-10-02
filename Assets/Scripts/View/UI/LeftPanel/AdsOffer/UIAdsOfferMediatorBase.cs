using Data;
using Holders;
using Infra.Instance;
using Model.AdsOffer;

namespace View.UI.LeftPanel.AdsOffer
{
    public abstract class UIAdsOfferMediatorBase<TViewModel> : MediatorWithModelBase<TViewModel>
        where TViewModel : AdsOfferViewModelBase
    {
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private string _secondsPostfix;
        private UIAdsOfferViewBase _view;

        protected virtual void Subscribe()
        {
            TargetModel.OfferTimeLeftChanged += OnOfferTimeLeftChanged;
        }
        
        protected virtual void Unsubscribe()
        {
            TargetModel.OfferTimeLeftChanged -= OnOfferTimeLeftChanged;
        }

        private void OnOfferTimeLeftChanged(int timeLeft)
        {
            if (_view == null) return;

            DisplayTimeLeft(_view);
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
    }
}