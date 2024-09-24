using System;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Common
{
    public class UISimpleButtonView : MonoBehaviour
    {
        public event Action Clicked;
        
        [SerializeField] private Button _button;
        [SerializeField] private Image _buttonBgImage;
        [SerializeField] private Image _buttonIconImage;

        public RectTransform RectTransform { get; private set; }
        public bool IsVisible => _button.gameObject.activeSelf;

        private void Awake()
        {
            RectTransform = transform as RectTransform;
            
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
        
        public void SetBgSprite(Sprite sprite)
        {
            _buttonBgImage.sprite = sprite;
        }

        public void SetIconSprite(Sprite sprite)
        {
            _buttonIconImage.sprite = sprite;
        }
        
        public void SetVisibility(bool isVisible)
        {
            _button.gameObject.SetActive(isVisible);
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }

        private void OnClick()
        {
            Clicked?.Invoke();
        }
    }
}