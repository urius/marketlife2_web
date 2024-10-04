using Data;
using Model.AdsOffer;

namespace View.UI.LeftPanel.AdsOffer
{
    public class UIAdsOfferAddMoneyMediator : UIAdsOfferMediatorBase<AdsOfferAddMoneyViewModel>
    {
        
        private UIAdsOfferAddMoneyView _offerView;

        protected override void MediateInternal()
        {
            _offerView = InstantiatePrefab<UIAdsOfferAddMoneyView>(PrefabKey.UIAdsOfferMoney);
            
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

        private void SetupView(UIAdsOfferAddMoneyView adsOfferView)
        {
            base.SetupView(adsOfferView);

            adsOfferView.SetRewardText($"+{TargetModel.MoneyAmountToAdd}");
        }
    }
}