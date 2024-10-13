using System;
using UnityEngine;
using UnityEngine.UI;
using View.UI.Common;
using View.UI.Popups.TabbedContentPopup;

namespace View.UI.Popups.DressesPopup
{
    public class UIDressesPopupItemView : MonoBehaviour, IUITabbedContentPopupItem
    {
        public event Action<UIDressesPopupItemView> ButtonClicked;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private UITextButtonView _button;
        [SerializeField] private Image _lockImage;
        [SerializeField] private Transform _newNotificationTransform;
        [SerializeField] private RectTransform _contentTransform;

        [Space(25)] 
        [SerializeField] private GameObject _itemContentHeadPrefab;
        [SerializeField] private GameObject _itemContentTopPrefab;
        [SerializeField] private GameObject _itemContentBottomPrefab;

        private UIDressesPopupItemContentUpperDressView _topDressView;
        private UIDressesPopupItemContentBottomDressView _bottomDressView;
        private UIDressesPopupItemContentHeadView _headContentView;
        private Sprite _hairDefaultSprite;

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
        
        public void SetupTopDress(Sprite primarySprite, Sprite secondarySprite)
        {
            DestroyHeadContentIfNeeded();
            DestroyBottomDressContentIfNeeded();
            
            if (_topDressView == null)
            {
                var go = Instantiate(_itemContentTopPrefab, _contentTransform);
                _topDressView = go.GetComponent<UIDressesPopupItemContentUpperDressView>();
            }

            _topDressView.BaseDressImage.sprite = primarySprite;
            _topDressView.HandDressImage.sprite = secondarySprite;
        }
        
        public void SetupBottomDress(Sprite sprite)
        {
            DestroyHeadContentIfNeeded();
            DestroyTopDressContentIfNeeded();
            
            if (_bottomDressView == null)
            {
                var go = Instantiate(_itemContentBottomPrefab, _contentTransform);
                _bottomDressView = go.GetComponent<UIDressesPopupItemContentBottomDressView>();
            }

            _bottomDressView.Leg1DressImage.sprite = sprite;
            _bottomDressView.Leg2DressImage.sprite = sprite;
        }
        
        public void SetupHair(Sprite sprite)
        {
            DestroyTopDressContentIfNeeded();
            DestroyBottomDressContentIfNeeded();
            
            CreatHeadContentViewIfNeeded();

            _headContentView.HairImage.sprite = sprite;
        }
        
        public void SetupGlasses(Sprite sprite)
        {
            DestroyTopDressContentIfNeeded();
            DestroyBottomDressContentIfNeeded();

            CreatHeadContentViewIfNeeded();

            _headContentView.GlassesImage.gameObject.SetActive(sprite != null);
            _headContentView.GlassesImage.sprite = sprite;
        }

        private void CreatHeadContentViewIfNeeded()
        {
            if (_headContentView == null)
            {
                var go = Instantiate(_itemContentHeadPrefab, _contentTransform);
                _headContentView = go.GetComponent<UIDressesPopupItemContentHeadView>();
                _hairDefaultSprite = _headContentView.HairImage.sprite;
            }
        }

        public void SetPosition(Vector2 position)
        {
            _rectTransform.anchoredPosition = position;
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

        private void DestroyHeadContentIfNeeded()
        {
            if (_headContentView != null)
            {
                Destroy(_headContentView.gameObject);
                _headContentView = null;
            }
        }

        private void DestroyTopDressContentIfNeeded()
        {
            if (_topDressView != null)
            {
                Destroy(_topDressView.gameObject);
                _topDressView = null;
            }
        }

        private void DestroyBottomDressContentIfNeeded()
        {
            if (_bottomDressView != null)
            {
                Destroy(_bottomDressView.gameObject);
                _bottomDressView = null;
            }
        }
    }
}