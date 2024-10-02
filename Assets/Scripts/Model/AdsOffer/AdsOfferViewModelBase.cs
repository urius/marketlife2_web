using System;
using Data;

namespace Model.AdsOffer
{
    public abstract class AdsOfferViewModelBase
    {
        public event Action<int> OfferTimeLeftChanged;
        
        public readonly int OfferTimeLeftMax;

        protected AdsOfferViewModelBase(int offerTimeLeft)
        {
            OfferTimeLeft = offerTimeLeft;
            OfferTimeLeftMax = OfferTimeLeft;
        }

        public abstract AdsOfferType AdsOfferType { get; }
        public int OfferTimeLeft { get; private set; }

        
        public void SetOfferTimeLeft(int time)
        {
            if (time == OfferTimeLeft) return;

            OfferTimeLeft = time;

            OfferTimeLeftChanged?.Invoke(OfferTimeLeft);
        }
    }
}