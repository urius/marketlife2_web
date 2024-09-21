using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Popups
{
    public class UITabbedContentPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private RectTransform _popupTransform;
        [SerializeField] private RectTransform _tabsButtonTransform;
        [SerializeField] private RectTransform _contentTransform;
        [SerializeField] private Button _closeButton;

        private LinkedList<RectTransform> _displayedItems;

        public RectTransform ContentTransform => _contentTransform;
        
        public void AddItem(RectTransform itemTransform)
        {
            if (_displayedItems.Count == 0)
            {
                _displayedItems.AddFirst(itemTransform);
            }
            else
            {
                for (var recentNode = _displayedItems.First;
                     recentNode != null;
                     recentNode = recentNode.Next)
                {
                    if (CheckCoordsForInsert(itemTransform, recentNode))
                    {
                        _displayedItems.AddBefore(recentNode, itemTransform);
                        
                        return;
                    }
                }

                _displayedItems.AddLast(itemTransform);
            }
        }

        private static bool CheckCoordsForInsert(RectTransform itemTransform, LinkedListNode<RectTransform> recentNode)
        {
            return itemTransform.anchoredPosition.x <= recentNode.Value.anchoredPosition.x;
        }

        public void SetTitleText(string text)
        {
            _titleText.text = text;
        }
        
        public void SetPopupSize(int width, int height)
        {
            _popupTransform.sizeDelta = new Vector2(width, height);
        }
        
        
        public void SetContentHeight(float height)
        {
            var tempSize = _popupTransform.sizeDelta;
            _contentTransform.sizeDelta = new Vector2(tempSize.x, height);
        }

        public void ClearContent()
        {
            foreach (Transform child in _contentTransform)
            {
                Destroy(child.gameObject);
            }

            SetContentHeight(0);
        }

        public void ResetContentPosition()
        {
            _contentTransform.anchoredPosition = Vector2.zero;
        }
    }
}