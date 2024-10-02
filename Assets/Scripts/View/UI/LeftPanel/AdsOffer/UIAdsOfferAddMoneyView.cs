using TMPro;
using UnityEngine;

namespace View.UI.LeftPanel.AdsOffer
{
    public class UIAdsOfferAddMoneyView : UIAdsOfferViewBase
    {
        [SerializeField] private TMP_Text _rewardText;

        public void SetRewardText(string text)
        {
            _rewardText.text = text;
        }
    }
}