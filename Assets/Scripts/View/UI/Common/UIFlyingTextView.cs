using TMPro;
using UnityEngine;

namespace View.UI.Common
{
    public sealed class UIFlyingTextView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Color _greenColor;
        [SerializeField] private Color _redColor;
        
        private Color _defaultColor;

        public RectTransform RectTransform => transform as RectTransform;

        private void Awake()
        {
            _defaultColor = _text.color;
        }

        private void OnEnable()
        {
            SetAlpha(1);
            SetTextColor(_defaultColor);
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetTextGreen()
        {
            SetTextColor(_greenColor);
        }

        public void SetTextRed()
        {
            SetTextColor(_redColor);
        }

        public void SetTextWhite()
        {
            SetTextColor(Color.white);
        }

        public void SetTextColor(Color color)
        {
            _text.color = color;
        }

        public void SetAlpha(float alpha)
        {
            _text.alpha = alpha;
        }
    }
}