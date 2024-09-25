using System;
using UnityEngine;
using View.Extensions;

namespace View.Game.People
{
    public class PlayerCompassView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _arrowImage;

        private float _arrowDistanceDefault;
        private float _arrowAlphaDefault;
        
        private void Awake()
        {
            _arrowDistanceDefault = _arrowImage.transform.localPosition.y;
            _arrowAlphaDefault = _arrowImage.color.a;
            
            transform.localPosition = Vector3.zero;
        }

        public void SetArrowDistancePercent(float percent)
        {
            var arrowImageTransform = _arrowImage.transform;
            
            var localPos = arrowImageTransform.localPosition;
            localPos.y = Math.Clamp(percent, 0, 1) * _arrowDistanceDefault;

            arrowImageTransform.localPosition = localPos;
        }
        
        public void SetArrowAlphaPercent(float percent)
        {
            var alpha = Math.Clamp(percent, 0, 1) * _arrowAlphaDefault;

            _arrowImage.SetAlpha(alpha);
        }

        public void SetPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }

        public void SetLookToPosition(Vector3 targetLookToPos)
        {
            var vectorRotation = targetLookToPos - transform.position;
            
            var angle = Vector2.SignedAngle(vectorRotation, new Vector2(-1, 1));

            var selfTransform = transform;
            var localEulerAngles = selfTransform.localEulerAngles;
            localEulerAngles.z = angle;
            selfTransform.localEulerAngles = localEulerAngles;
        }
    }
}