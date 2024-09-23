using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Popups.TabbedContentPopup
{
    public class UITabbedContentPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private RectTransform _popupTransform;
        [SerializeField] private RectTransform _tabsButtonTransform;
        [SerializeField] private RectTransform _viewportTransform;
        [SerializeField] private RectTransform _contentTransform;
        [SerializeField] private Button _closeButton;

        private readonly LinkedList<ItemData> _hiddenItemsHead = new();
        private readonly LinkedList<ItemData> _displayedItems = new();
        private readonly LinkedList<ItemData> _hiddenItemsTail = new();
        
        private int _columnsCount = 3;
        private Vector2 _contentSize;
        private Vector2 _viewPortSize;
        private Vector2 _contentTransformPosition;

        public RectTransform ContentTransform => _contentTransform;
        private void Start()
        {
            _contentTransformPosition = _contentTransform.anchoredPosition;
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

        public void Setup(int columnsCount, int popupWidth, int popupHeight)
        {
            _viewPortSize = _viewportTransform.rect.size;
            
            _columnsCount = columnsCount;
            SetPopupSize(popupWidth, popupHeight);
        }
        
        public void AddItem(IUITabbedContentPopupItem item)
        {
            item.RectTransform.SetParent(_contentTransform);
            
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

            _displayedItems.Clear();
            SetContentHeight(0);
        }

        public void ResetContentPosition()
        {
            _contentTransform.anchoredPosition = Vector2.zero;
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
                    lastItemData.Item.RectTransform.gameObject.SetActive(false);

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
                    firstHiddenItemData.Item.RectTransform.gameObject.SetActive(true);

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
                    firstItemData.Item.RectTransform.gameObject.SetActive(false);

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
                    lastHiddenItemData.Item.RectTransform.gameObject.SetActive(true);

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

        public void SetTitleText(string text)
        {
            _titleText.text = text;
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