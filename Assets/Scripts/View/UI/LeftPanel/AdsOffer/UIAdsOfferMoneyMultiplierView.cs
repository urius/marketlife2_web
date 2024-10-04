using TMPro;
using UnityEngine;

namespace View.UI.LeftPanel.AdsOffer
{
    public class UIAdsOfferMoneyMultiplierView : UIAdsOfferViewBase
    {
        [SerializeField] private TMP_Text _multiplierValueText;

        public void SetRewardTimeText(string text)
        {
            SetButtonText(text);
        }
        
        public void SetMultiplierValueText(string text)
        {
            _multiplierValueText.text = text;
        }
    }
}