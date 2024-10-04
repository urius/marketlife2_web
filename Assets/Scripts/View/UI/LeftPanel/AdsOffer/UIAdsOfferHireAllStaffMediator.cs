using Data;
using Holders;
using Infra.Instance;
using Model.AdsOffer;

namespace View.UI.LeftPanel.AdsOffer
{
    public class UIAdsOfferHireAllStaffMediator : UIAdsOfferMediatorBase<AdsOfferHireAllStaffViewModel>
    {
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private UIAdsOfferHireAllStaffView _offerView;

        protected override void MediateInternal()
        {
            _offerView = InstantiatePrefab<UIAdsOfferHireAllStaffView>(PrefabKey.UIAdsOfferHireAllStaff);

            SetupView(_offerView);

            _offerView.AnimateAppearingFromLeft();

            Subscribe();
        }

        private void SetupView(UIAdsOfferHireAllStaffView adsOfferView)
        {
            base.SetupView(adsOfferView);

            var hireAllStaffText = _localizationProvider.GetLocale(Constants.LocalizationKeyAdsOfferHireAllStaff);
            adsOfferView.SetDescriptionText(hireAllStaffText);
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_offerView);
            _offerView = null;
        }
    }
}