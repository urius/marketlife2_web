using TMPro;
using UnityEngine;

namespace View.UI.TopPanel
{
    public class UITopPanelLevelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private TMP_Text _levelStatusText;
        [SerializeField] private TMP_Text _newLevelText;
        [SerializeField] private RectTransform _progressBarTransform;
        [SerializeField] private RectTransform _starTransform;
        [SerializeField] private bool _isNewLevelTextVisible = false;

        public RectTransform StarTransform => _starTransform;
        public RectTransform RectTransform => transform as RectTransform;
        
        private Vector2 _defaultProgressBarSizeDelta;
        private float _newLevelTextAlphaExtraOffset;
        private bool _animateNewLevelTextFadingOutFlag = false;

        private void Awake()
        {
            _defaultProgressBarSizeDelta = _progressBarTransform.sizeDelta;

            SetNewLevelTextVisibility(false);
        }
        
        private void FixedUpdate()
        {
            if (_isNewLevelTextVisible)
            {
                ChangeNewLevelTextAlphaOverTime();
            }
        }

        public void SetProgressBarRatio(float value)
        {
            _progressBarTransform.sizeDelta =
                new Vector3(value * _defaultProgressBarSizeDelta.x, _defaultProgressBarSizeDelta.y);
        }

        public void SetLevelText(string text)
        {
            _levelText.text = text;
        }

        public void SetProgressText(string text)
        {
            _progressText.text = text;
        }

        public void SetLevelStatusText(string levelStatusText)
        {
            _levelStatusText.text = levelStatusText;
        }

        public void SetVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetNewLevelText(string text)
        {
            _newLevelText.text = text;
        }

        public void SetNewLevelTextVisibility(bool isVisible)
        {
            _isNewLevelTextVisible = isVisible;
            _newLevelText.gameObject.SetActive(_isNewLevelTextVisible);
            
            _animateNewLevelTextFadingOutFlag = false;
            _newLevelTextAlphaExtraOffset = 0;
        }

        public void AnimateNewLevelTextFadingOut()
        {
            _animateNewLevelTextFadingOutFlag = true;
            _newLevelTextAlphaExtraOffset = 0;
        }

        private void ChangeNewLevelTextAlphaOverTime()
        {
            var newAlpha = Mathf.PingPong(Time.time * 3, 1f) + 0.5f + _newLevelTextAlphaExtraOffset;
            var newColor = _newLevelText.color;
            newColor.a = newAlpha;
            _newLevelText.color = newColor;

            if (_animateNewLevelTextFadingOutFlag)
            {
                _newLevelTextAlphaExtraOffset -= 0.05f;
                if (_newLevelTextAlphaExtraOffset < -1)
                {
                    SetNewLevelTextVisibility(false);
                }
            }
        }
    }
}