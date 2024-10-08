using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.Extensions;

namespace View.UI.Popups.TabbedContentPopup
{
    public class UITabbedContentPopup : MonoBehaviour
    {
        public event Action CloseButtonClicked;
        public event Action<int> TabButtonClicked;

        private const float AppearDurationSec = 0.4f;
        private const float DisappearDurationSec = 0.25f;

        [SerializeField] private Image _blockRaycastsImage;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private RectTransform _popupTransform;
        [SerializeField] private CanvasGroup _popupBodyCanvasGroup;
        [SerializeField] private RectTransform _tabsButtonsTransform;
        [SerializeField] private RectTransform _viewportTransform;
        [SerializeField] private RectTransform _contentTransform;
        [SerializeField] private Button _closeButton;
        [SerializeField] private GameObject _tabButtonPrefab;

        private readonly LinkedList<ItemData> _hiddenItemsHead = new();
        private readonly LinkedList<ItemData> _displayedItems = new();
        private readonly LinkedList<ItemData> _hiddenItemsTail = new();
        private readonly List<IUITabbedContentPopupTabButton> _tabButtons = new();

        private int _columnsCount = 3;
        private Vector2 _contentSize;
        private Vector2 _viewPortSize;
        private Vector2 _contentTransformPosition;

        public RectTransform ContentTransform => _contentTransform;
        public IReadOnlyList<IUITabbedContentPopupTabButton> TabButtons => _tabButtons;

        private void Awake()
        {
            _contentTransformPosition = _contentTransform.anchoredPosition;

            _closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        private void Update()
        {
            var newContentPosition = _contentTransform.anchoredPosition;

            if (newContentPosition.y < _contentTransformPosition.y)
            {
                ProcessScrollForward();
            }
            else if (newContentPosition.y > _contentTransformPosition.y)
            {
                ProcessScrollBackward();
            }

            _contentTransformPosition = newContentPosition;
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveAllListeners();

            foreach (var tabButton in _tabButtons)
            {
                UnsubscribeFromTabButton(tabButton);
            }

            _tabButtons.Clear();
        }

        public void Setup(int columnsCount, int popupWidth, int popupHeight)
        {
            _viewPortSize = _viewportTransform.rect.size;

            _columnsCount = columnsCount;
            SetPopupSize(popupWidth, popupHeight);
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

            //AudioManager.Instance.PlaySound(SoundNames.PopupOpen);
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

            //AudioManager.Instance.PlaySound(SoundNames.PopupClose);
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

            //AudioManager.Instance.PlaySound(SoundNames.PopupOpen);
            return tcs.Task;
        }

        public UniTask Disappear2Async()
        {
            //AudioManager.Instance.PlaySound(SoundNames.PopupClose);

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

        public void AddTab(string tabTitle) //AddBab :)
        {
            var tabGo = Instantiate(_tabButtonPrefab, _tabsButtonsTransform);
            var tabButtonView = tabGo.GetComponent<IUITabbedContentPopupTabButton>();

            tabButtonView.SetText(tabTitle);

            var itemPos = tabButtonView.RectTransform.anchoredPosition;
            itemPos.x = _tabButtons.Count * tabButtonView.RectTransform.rect.width;
            tabButtonView.RectTransform.anchoredPosition = itemPos;

            _tabButtons.Add(tabButtonView);

            SubscribeOnTabButton(tabButtonView, _tabButtons.Count - 1);
        }

        public void AddItem(IUITabbedContentPopupItem item)
        {
            item.RectTransform.SetParent(_contentTransform);
            SetItemActive(item, true);

            var allItemsCount = _hiddenItemsHead.Count + _displayedItems.Count + _hiddenItemsTail.Count;
            SetItemPosition(item, allItemsCount);

            var itemData = new ItemData(item);

            if (-itemData.EndCoord > _contentSize.y)
            {
                SetContentHeight(-itemData.EndCoord);
            }

            _displayedItems.AddLast(new ItemData(item));

            TryHideTailItem();
        }

        public void SetPopupSize(int width, int height)
        {
            _popupTransform.sizeDelta = new Vector2(width, height);
        }

        public void ClearContent()
        {
            foreach (Transform child in _contentTransform)
            {
                Destroy(child.gameObject);
            }

            _hiddenItemsHead.Clear();
            _displayedItems.Clear();
            _hiddenItemsTail.Clear();

            SetContentHeight(0);
        }

        public void ResetContentPosition()
        {
            _contentTransform.anchoredPosition = Vector2.zero;
        }

        public void SetSelectedTab(int tabIndex)
        {
            foreach (var tabButton in _tabButtons)
            {
                tabButton.Button.interactable = true;
            }

            _tabButtons[tabIndex].Button.interactable = false;
        }

        public void SetTabNewNotificationVisibility(int tabIndex, bool isVisible)
        {
            _tabButtons[tabIndex].SetNewNotificationVisibility(isVisible);
        }

        public void SetTitleText(string text)
        {
            _titleText.text = text;
        }

        private void ProcessScrollForward()
        {
            ProcessItemsAction(TryShowHeadHiddenItem);
            ProcessItemsAction(TryHideTailItem);
        }

        private void ProcessScrollBackward()
        {
            ProcessItemsAction(TryShowTailHiddenItem);
            ProcessItemsAction(TryHideHeadItem);
        }

        private static void ProcessItemsAction(Func<bool> action)
        {
            var safeCounter = 100;

            while (action() && safeCounter > 0)
            {
                safeCounter--;
            }

            if (safeCounter <= 0)
            {
                Debug.LogError($"{nameof(safeCounter)} alert!");
            }
        }

        private bool TryHideTailItem()
        {
            if (_displayedItems.Count > 0)
            {
                var lastItemData = _displayedItems.Last.Value;

                if (ShouldHideItemAtTail(lastItemData))
                {
                    SetItemActive(lastItemData.Item, false);

                    _displayedItems.RemoveLast();
                    _hiddenItemsTail.AddFirst(lastItemData);

                    return true;
                }
            }

            return false;
        }

        private bool TryShowTailHiddenItem()
        {
            if (_hiddenItemsTail.Count > 0)
            {
                var firstHiddenItemData = _hiddenItemsTail.First.Value;

                if (ShouldHideItemAtTail(firstHiddenItemData) == false)
                {
                    SetItemActive(firstHiddenItemData.Item, true);

                    _hiddenItemsTail.RemoveFirst();
                    _displayedItems.AddLast(firstHiddenItemData);

                    return true;
                }
            }

            return false;
        }

        private bool TryHideHeadItem()
        {
            if (_displayedItems.Count > 0)
            {
                var firstItemData = _displayedItems.First.Value;

                if (ShouldHideItemAtHead(firstItemData))
                {
                    SetItemActive(firstItemData.Item, false);

                    _displayedItems.RemoveFirst();
                    _hiddenItemsHead.AddLast(firstItemData);

                    return true;
                }
            }

            return false;
        }

        private bool TryShowHeadHiddenItem()
        {
            if (_hiddenItemsHead.Count > 0)
            {
                var lastHiddenItemData = _hiddenItemsHead.Last.Value;

                if (ShouldHideItemAtHead(lastHiddenItemData) == false)
                {
                    SetItemActive(lastHiddenItemData.Item, true);

                    _hiddenItemsHead.RemoveLast();
                    _displayedItems.AddFirst(lastHiddenItemData);

                    return true;
                }
            }

            return false;
        }

        private bool ShouldHideItemAtTail(ItemData itemData)
        {
            return -(itemData.StartCoord + _contentTransformPosition.y) > _viewPortSize.y;
        }

        private bool ShouldHideItemAtHead(ItemData itemData)
        {
            return -(itemData.EndCoord + _contentTransformPosition.y) < 0;
        }

        private Vector2 SetItemPosition(IUITabbedContentPopupItem item, int itemIndex)
        {
            var position = new Vector2Int(itemIndex % _columnsCount, -itemIndex / _columnsCount) * item.Size;

            item.RectTransform.anchoredPosition = position;

            return position;
        }

        private void SetContentHeight(float height)
        {
            var tempSize = _popupTransform.sizeDelta;
            _contentTransform.sizeDelta = new Vector2(tempSize.x, height);
            _contentSize = _contentTransform.sizeDelta;
        }

        private void OnCloseButtonClick()
        {
            CloseButtonClicked?.Invoke();
        }

        private void SubscribeOnTabButton(IUITabbedContentPopupTabButton tabButtonView, int index)
        {
            tabButtonView.Button.onClick.AddListener(() => OnTabButtonClicked(index));
        }

        private void UnsubscribeFromTabButton(IUITabbedContentPopupTabButton tabButtonView)
        {
            tabButtonView.Button.onClick.RemoveAllListeners();
        }

        private void OnTabButtonClicked(int index)
        {
            TabButtonClicked?.Invoke(index);
        }

        private static void SetItemActive(IUITabbedContentPopupItem item, bool isActive)
        {
            item.RectTransform.gameObject.SetActive(isActive);
        }

        private struct ItemData
        {
            public readonly IUITabbedContentPopupItem Item;
            public readonly float StartCoord;
            public readonly float EndCoord;

            public ItemData(IUITabbedContentPopupItem item)
            {
                Item = item;

                var anchoredPosition = Item.RectTransform.anchoredPosition;
                StartCoord = anchoredPosition.y;
                EndCoord = anchoredPosition.y - Item.Size.y;
            }
        }
    }
}