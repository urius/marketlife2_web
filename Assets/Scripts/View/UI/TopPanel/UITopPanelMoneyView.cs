using TMPro;
using UnityEngine;

namespace View.UI.TopPanel
{
    public class UITopPanelMoneyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        
        private Color _textDefaultColor;

        private void Awake()
        {
            _textDefaultColor = _text.color;
        }

        public void SetMoneyText(string text)
        {
            _text.text = text;
        }

        public void SetTextColor(Color color)
        {
            _text.color = color;
        }
    }
}