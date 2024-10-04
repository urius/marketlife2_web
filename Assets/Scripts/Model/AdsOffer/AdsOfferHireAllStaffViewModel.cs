using Data;

namespace Model.AdsOffer
{
    public class AdsOfferHireAllStaffViewModel : AdsOfferViewModelBase
    {
        public AdsOfferHireAllStaffViewModel(int offerTimeLeft) : base(offerTimeLeft)
        {
        }

        public override AdsOfferType AdsOfferType => AdsOfferType.HireAllStaff;
    }
}