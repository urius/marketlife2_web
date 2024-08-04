using UnityEngine;

namespace View.UI.BottomPanel
{
    public class UIBottomPanelViewBase : MonoBehaviour
    {
        [SerializeField] private Canvas _mainCanvas;
        
        private RectTransform _rectTransform;
        private float _slideDownYPosition;
        
        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _slideDownYPosition = _rectTransform.anchoredPosition.y;
        }

        public float SetSlideUpPositionPercent(float positionPercent)
        {
            var yPos = Mathf.Lerp(_slideDownYPosition, 0, positionPercent);
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, yPos);

            return yPos;
        }

        public void SetActive(bool isActive)
        {
            _mainCanvas.enabled = isActive;
            gameObject.SetActive(isActive);
        }
    }
}