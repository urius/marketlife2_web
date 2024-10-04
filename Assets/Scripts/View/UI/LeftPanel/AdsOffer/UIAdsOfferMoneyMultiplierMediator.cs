using Data;
using Holders;
using Infra.Instance;
using Model.AdsOffer;

namespace View.UI.LeftPanel.AdsOffer
{
    public class UIAdsOfferMoneyMultiplierMediator : UIAdsOfferMediatorBase<AdsOfferMoneyMultiplierViewModel>
    {
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private UIAdsOfferMoneyMultiplierView _offerView;

        protected override void MediateInternal()
        {
            _offerView = InstantiatePrefab<UIAdsOfferMoneyMultiplierView>(PrefabKey.UIAdsOfferMoneyMultiplier);
            
            SetupView(_offerView);
            
            _offerView.AnimateAppearingFromLeft();
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_offerView);
            _offerView = null;
        }
        
        private void SetupView(UIAdsOfferMoneyMultiplierView adsOfferView)
        {
            base.SetupView(adsOfferView);

            adsOfferView.SetMultiplierValueText($"x{TargetModel.Multiplier}");
            adsOfferView.SetRewardTimeText($"{TargetModel.RewardTimeMin} {_localizationProvider.GetLocale(Constants.LocalizationKeyMinutesShort)}");
        }
    }
}