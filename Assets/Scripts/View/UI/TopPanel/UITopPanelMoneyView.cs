using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace View.UI.TopPanel
{
    public class UITopPanelMoneyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _moneyIconTransform;
        [SerializeField] private UIMoneyEarnMultiplierModifierView _moneyEarnMultiplierModifierView;
        
        private Color _textDefaultColor;
        private Vector2 _defaultMoneyIconPosition;
        private CancellationTokenSource _moneyIconBounceTcs;

        public Color TextDefaultColor => _textDefaultColor;
        public UIMoneyEarnMultiplierModifierView MoneyEarnMultiplierModifierView => _moneyEarnMultiplierModifierView;

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

        public UniTask AnimateIconJump()
        {
            _moneyIconBounceTcs?.Cancel();
            _moneyIconBounceTcs = new CancellationTokenSource();
            
            ResetMoneyIconPosition();
            
            return LeanTweenHelper.BounceYAsync(_moneyIconTransform, deltaY: -25f, duration1: 0.3f, duration2: 0.6f,
                stopToken: _moneyIconBounceTcs.Token, ignoreTimeScale: true);
        }
    }
}