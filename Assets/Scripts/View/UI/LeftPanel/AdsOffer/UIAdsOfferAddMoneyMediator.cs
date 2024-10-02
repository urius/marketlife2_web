using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using Model.AdsOffer;

namespace View.UI.LeftPanel.AdsOffer
{
    public class UIAdsOfferAddMoneyMediator : UIAdsOfferMediatorBase<AdsOfferAddMoneyViewModel>
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
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

        protected override void Subscribe()
        {
            base.Subscribe();

            TargetModel.OfferTimeLeftChanged += OnOfferTimeLeftChanged;
            _offerView.ButtonClick += OnButtonClick;
        }

        protected override void Unsubscribe()
        {
            TargetModel.OfferTimeLeftChanged -= OnOfferTimeLeftChanged;
            _offerView.ButtonClick -= OnButtonClick;

            base.Unsubscribe();
        }

        private void OnButtonClick()
        {
            _eventBus.Dispatch(new AdsOfferClickedEvent(TargetModel));
        }

        private void OnOfferTimeLeftChanged(int timeLeft)
        {
            if (timeLeft == 0)
            {
                _offerView.AnimateDisappearingToLeft();
            }
        }
    }
}