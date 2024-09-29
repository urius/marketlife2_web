using System;
using UnityEngine;
using UnityEngine.UI;
using View.UI.Common;
using View.UI.Popups.TabbedContentPopup;

namespace View.UI.Popups.InteriorPopup
{
    public class UIInteriorPopupItemView : MonoBehaviour, IUITabbedContentPopupItem
    {
        public event Action<UIInteriorPopupItemView> ButtonClicked;
            
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private UITextButtonView _button;
        [SerializeField] private Image _itemImage;
        [SerializeField] private Image _lockImage;
        [SerializeField] private Transform _newNotificationTransform;

        public RectTransform RectTransform => _rectTransform;
        public Vector2 Size => _rectTransform.sizeDelta;
        public UITextButtonView Button => _button;

        private void Awake()
        {
            SetNewNotificationVisibility(false);

            Button.Button.onClick.AddListener(OnBuyButtonClick);
        }

        private void OnDestroy()
        {
            Button.Button.onClick.RemoveAllListeners();
        }

        public void SetPosition(Vector2 position)
        {
            _rectTransform.anchoredPosition = position;
        }

        public void SetItemSprite(Sprite sprite)
        {
            _itemImage.sprite = sprite;
        }

        public void SetLockVisibility(bool isVisible)
        {
            _lockImage.gameObject.SetActive(isVisible);
        }

        public void SetButtonInteractable(bool isInteractable)
        {
            _button.SetInteractable(isInteractable);
        }

        public void SetButtonText(string text)
        {
            _button.SetText(text);
        }

        public void SetNewNotificationVisibility(bool isVisible)
        {
            _newNotificationTransform.gameObject.SetActive(isVisible);
        }

        private void OnBuyButtonClick()
        {
            ButtonClicked?.Invoke(this);
        }
    }
}