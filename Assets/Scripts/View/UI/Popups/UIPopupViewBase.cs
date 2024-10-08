using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.Extensions;

namespace View.UI.Popups
{
    public class UIPopupViewBase : MonoBehaviour
    {
        public event Action CloseButtonClicked;
        
        private const float AppearDurationSec = 0.4f;
        private const float DisappearDurationSec = 0.25f;
        
        [SerializeField] private Image _blockRaycastsImage;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private RectTransform _popupTransform;
        [SerializeField] private CanvasGroup _popupBodyCanvasGroup;
        [SerializeField] private Button _closeButton;
        
        protected virtual void Awake()
        {
            _closeButton.onClick.AddListener(OnCloseButtonClick);
        }
        
        protected virtual void OnDestroy()
        {
            _closeButton.onClick.RemoveAllListeners();
        }

        public void SetCloseButtonVisibility(bool isVisible)
        {
            _closeButton.gameObject.SetActive(isVisible);
        }
        
        public void SetTitleText(string text)
        {
            _titleText.text = text;
        }
        
        public void SetPopupSize(int width, int height)
        {
            _popupTransform.sizeDelta = new Vector2(width, height);
        }
        
        public UniTask AppearAsync()
        {
            var tcs = new UniTaskCompletionSource();
            _popupBodyCanvasGroup.alpha = 0;
            LeanTween.value(gameObject, a => _popupBodyCanvasGroup.alpha = a, 0, 1, 0.5f * AppearDurationSec)
                .setIgnoreTimeScale(true);
            LeanTween.value(gameObject, p => _popupTransform.anchoredPosition = p, new Vector2(0, -300), Vector2.zero,
                    AppearDurationSec)
                .setEaseOutBack()
                .setOnComplete(() => tcs.TrySetResult())
                .setIgnoreTimeScale(true);;

            return tcs.Task;
        }

        public UniTask DisappearAsync()
        {
            var tcs = new UniTaskCompletionSource();
            _blockRaycastsImage.SetAlpha(0);
            LeanTween.value(gameObject, a => _popupBodyCanvasGroup.alpha = a, 1, 0, DisappearDurationSec)
                .setIgnoreTimeScale(true);;
            LeanTween.value(gameObject, p => _popupTransform.anchoredPosition = p, Vector2.zero, new Vector2(0, 300),
                    DisappearDurationSec)
                .setEaseInBack()
                .setOnComplete(() => tcs.TrySetResult())
                .setIgnoreTimeScale(true);;

            return tcs.Task;
        }

        public UniTask Appear2Async()
        {
            var tcs = new UniTaskCompletionSource();
            var targetSize = _popupTransform.sizeDelta;
            var startSize = new Vector2(targetSize.x, 0);
            _popupBodyCanvasGroup.alpha = 0;
            LeanTween.value(gameObject, a => _popupBodyCanvasGroup.alpha = a, 0, 1, 0.5f * AppearDurationSec)
                .setIgnoreTimeScale(true);;
            LeanTween.value(gameObject, p => _popupTransform.sizeDelta = p, startSize, targetSize, AppearDurationSec)
                .setEaseOutBack()
                .setOnComplete(() => tcs.TrySetResult())
                .setIgnoreTimeScale(true);;

            return tcs.Task;
        }

        public UniTask Disappear2Async()
        {
            var tcs = new UniTaskCompletionSource();
            var startSize = _popupTransform.sizeDelta;
            var targetSize = new Vector2(startSize.x, 0);
            _blockRaycastsImage.SetAlpha(0);
            LeanTween.value(gameObject, a => _popupBodyCanvasGroup.alpha = a, 1, 0, DisappearDurationSec)
                .setIgnoreTimeScale(true);;
            LeanTween.value(gameObject, p => _popupTransform.sizeDelta = p, startSize, targetSize, DisappearDurationSec)
                .setEaseInBack()
                .setOnComplete(() => tcs.TrySetResult())
                .setIgnoreTimeScale(true);;
            
            return tcs.Task;
        }
        
        private void OnCloseButtonClick()
        {
            CloseButtonClicked?.Invoke();
        }
    }
}