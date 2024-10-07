using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace View.UI.TopPanel
{
    public class UIMoneyEarnMultiplierModifierView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _multiplierText;
        [SerializeField] private TMP_Text _timeLeftText;

        private RectTransform _rectTransform;
        private Vector2 _defaultPosition;
        private CancellationTokenSource _bounceAnimationTcs;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            _defaultPosition = _rectTransform.anchoredPosition;
        }

        public void SetMultiplierText(string text)
        {
            _multiplierText.text = text;
        }

        public void SetTimeLeftText(string text)
        {
            _timeLeftText.text = text;
        }
        
        public UniTask AnimateIconJump()
        {
            _bounceAnimationTcs?.Cancel();
            _bounceAnimationTcs = new CancellationTokenSource();
            
            ResetPosition();
            
            return LeanTweenHelper.BounceYAsync(_rectTransform, deltaY: -25f, duration1: 0.3f, duration2: 0.6f,
                stopToken: _bounceAnimationTcs.Token, ignoreTimeScale: true);
        }

        private void ResetPosition()
        {
            _rectTransform.anchoredPosition = _defaultPosition;
        }
    }
}