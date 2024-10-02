using Data;

namespace Model.AdsOffer
{
    public class AdsOfferAddMoneyViewModel : AdsOfferViewModelBase
    {
        public readonly int MoneyAmountToAdd;

        public AdsOfferAddMoneyViewModel(int offerTimeLeft, int moneyAmountToAdd) 
            : base(offerTimeLeft)
        {
            MoneyAmountToAdd = moneyAmountToAdd;
        }

        public override AdsOfferType AdsOfferType => AdsOfferType.AddMoney;
    }
}