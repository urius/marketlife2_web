using UnityEngine;

namespace View.UI.GameOverlayPanel.MovingControl
{
    public class UIMovingControlView : MonoBehaviour
    {
        [SerializeField] private RectTransform _innerPartTransform;
        
        private RectTransform _rectTransform;
        private float _radius;
        private float _normalizationMultiplier;
        private float _sqrRadius;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;

            _radius = _rectTransform.sizeDelta.x * 0.5f - _innerPartTransform.sizeDelta.x * 0.2f;
            _sqrRadius = _radius * _radius;
            _normalizationMultiplier = 1 / _radius;
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

        public void SetInnerPartPosition(Vector2 innerPartLocalPos)
        {
            var resultPos = innerPartLocalPos;

            if (innerPartLocalPos.sqrMagnitude >= _sqrRadius)
            {
                resultPos = innerPartLocalPos.normalized * _radius;
            }

            _innerPartTransform.anchoredPosition = resultPos;
        }
        
        public Vector2 GetInnerPartPosition()
        {
            return _innerPartTransform.anchoredPosition * _normalizationMultiplier;
        }
    }
}
