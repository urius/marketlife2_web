using UnityEngine;
using UnityEngine.UI;
using View.UI.Common;

namespace View.UI.Popups.InteriorPopup
{
    public class UIInteriorPopupItemView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private UITextButtonView _button;
        [SerializeField] private Image _itemImage;
        [SerializeField] private Image _lockImage;

        public RectTransform RectTransform => _rectTransform;
        public Vector2 Size => _rectTransform.sizeDelta;
        public UITextButtonView Button => _button;
        
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
    }
}