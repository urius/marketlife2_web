using TMPro;
using UnityEngine;

namespace View.UI.TopPanel
{
    public class UITopPanelLevelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private TMP_Text _levelStatusText;
        [SerializeField] private RectTransform _progressBarTransform;
        
        private Vector2 _defaultProgressBarSizeDelta;

        private void Awake()
        {
            _defaultProgressBarSizeDelta = _progressBarTransform.sizeDelta;
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
    }
}