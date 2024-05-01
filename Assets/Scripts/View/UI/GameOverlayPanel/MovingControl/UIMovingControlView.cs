using System;
using UnityEngine;

namespace View.UI.GameOverlayPanel.MovingControl
{
    public class UIMovingControlView : MonoBehaviour
    {
        [SerializeField] private RectTransform _innerPartTransform;
        
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
        }

        public void ResetAndActivate()
        {
            _innerPartTransform.localPosition = Vector3.zero;
            
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void SetAnchoredPosition(Vector2 pos)
        {
            _rectTransform.anchoredPosition = pos;
        }
    }
}
