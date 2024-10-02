using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.LeftPanel.AdsOffer
{
    public class UIAdsOfferViewBase : MonoBehaviour
    {
        private const float AppearingDuration = 0.3f;
        private const float DisappearingDuration = AppearingDuration;
        
        public event Action ButtonClick;
        
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private TMP_Text _timeLeftText;
        [SerializeField] private RectTransform _timeLeftProgressBarTransform;
        
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public UniTask AnimateAppearingFromLeft()
        {
            var anchoredPosition = _rectTransform.anchoredPosition;
            anchoredPosition.x = -200;
            _rectTransform.anchoredPosition = anchoredPosition;

            var (task, decr) = LeanTweenHelper.MoveXAsync(_rectTransform, 0, AppearingDuration);
            decr.setEaseOutBack()
                .setIgnoreTimeScale(true);;

            return task;
        }

        public UniTask AnimateDisappearingToLeft()
        {
            var (task, decr) = LeanTweenHelper.MoveXAsync(_rectTransform, -200, DisappearingDuration);
            decr.setEaseInBack()
                .setIgnoreTimeScale(true);

            return task;
        }

        public void SetTimeLeftProgress(float progress)
        {
            var scale = _timeLeftProgressBarTransform.localScale;
            scale.x = progress;
            _timeLeftProgressBarTransform.localScale = scale;
        }

        public void SetTimeLeftText(string text)
        {
            _timeLeftText.text = text;
        }


        public void SetButtonText(string text)
        {
            _buttonText.text = text;
        }

        private void OnButtonClick()
        {
            ButtonClick?.Invoke();
        }
    }
}