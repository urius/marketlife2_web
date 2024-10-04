using TMPro;
using UnityEngine;

namespace View.UI.TopPanel
{
    public class UIMoneyEarnMultiplierModifierView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _multiplierText;
        [SerializeField] private TMP_Text _timeLeftText;

        public void SetMultiplierText(string text)
        {
            _multiplierText.text = text;
        }

        public void SetTimeLeftText(string text)
        {
            _timeLeftText.text = text;
        }
    }
}