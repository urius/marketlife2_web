using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.TopPanel
{
    public class UITopPanelMoneyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _moneyIconTransform;
        
        private Color _textDefaultColor;
        private Vector2 _defaultMoneyIconPosition;

        public Color TextDefaultColor => _textDefaultColor;

        private void Awake()
        {
            _defaultMoneyIconPosition = _moneyIconTransform.anchoredPosition;
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
        
        public void SetDefaultTextColor()
        {
            _text.color = _textDefaultColor;
        }

        public void SetMoneyIconXOffset(float xOffset)
        {
            _moneyIconTransform.anchoredPosition = new Vector2(xOffset, _defaultMoneyIconPosition.y);
        }

        public void ResetMoneyIconPosition()
        {
            _moneyIconTransform.anchoredPosition = _defaultMoneyIconPosition;
        }
    }
}