using Data;

namespace Model.AdsOffer
{
    public class AdsOfferMoneyMultiplierViewModel : AdsOfferViewModelBase
    {
        public readonly int Multiplier;
        public readonly int RewardTimeMin;
        
        public AdsOfferMoneyMultiplierViewModel(int offerTimeLeft, int rewardTimeMin, int multiplier) 
            : base(offerTimeLeft)
        {
            RewardTimeMin = rewardTimeMin;
            Multiplier = multiplier;
        }

        public override AdsOfferType AdsOfferType => AdsOfferType.MoneyMultiplier;
    }
}