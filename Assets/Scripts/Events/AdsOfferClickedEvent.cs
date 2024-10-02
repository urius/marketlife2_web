using Model.AdsOffer;

namespace Events
{
    public struct AdsOfferClickedEvent
    {
        public readonly AdsOfferViewModelBase TargetModel;

        public AdsOfferClickedEvent(AdsOfferViewModelBase targetModel)
        {
            TargetModel = targetModel;
        }
    }
}