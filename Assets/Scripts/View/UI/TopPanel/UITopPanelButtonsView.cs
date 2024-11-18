using UnityEngine;

namespace View.UI.TopPanel
{
    public class UITopPanelButtonsView : MonoBehaviour
    {
        [SerializeField] private UITopPanelButtonView _interiorButton;
        [SerializeField] private UITopPanelButtonView _dressesButton;
        [SerializeField] private UITopPanelButtonView _leaderboardButton;

        public UITopPanelButtonView InteriorButton => _interiorButton;
        public UITopPanelButtonView DressesButton => _dressesButton;
        public UITopPanelButtonView LeaderboardButton => _leaderboardButton;
    }
}