using System;
using UnityEngine;
using View.Extensions;

namespace View.Game.People
{
    public class PlayerCompassView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _arrowImage;
        [SerializeField] private SpriteRenderer _iconImage;
        [Space(5)] 
        [SerializeField] private PresetData _cashDeskPreset;
        [SerializeField] private PresetData _truckPointPreset;
        [SerializeField] private PresetData _shelfPreset;

        private float _arrowDistanceDefault;
        private float _arrowAlphaDefault;
        private Quaternion _iconRotationDefault;
        private Transform _transform;
        private Transform _iconImageTransform;

        private void Awake()
        {
            _transform = transform;
            _iconImageTransform = _iconImage.transform;
            
            _arrowDistanceDefault = _arrowImage.transform.localPosition.y;
            _arrowAlphaDefault = _arrowImage.color.a;
            
            _iconRotationDefault = _iconImageTransform.rotation;
            
            _transform.localPosition = Vector3.zero;
        }

        public void SetCashDeskPreset()
        {
            SetPreset(_cashDeskPreset);
        }

        public void SetTruckPointPreset()
        {
            SetPreset(_truckPointPreset);
        }

        public void SetShelfPreset()
        {
            SetPreset(_shelfPreset);
        }

        private void SetPreset(PresetData presetData)
        {
            _iconImage.sprite = presetData.IconImageSprite;
            
            _iconImage.SetColorWithoutAlpha(presetData.Color);
            _arrowImage.SetColorWithoutAlpha(presetData.Color);
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
            var vectorRotation = targetLookToPos - _transform.position;
            
            var angle = Vector2.SignedAngle(vectorRotation, new Vector2(-1, 1));

            var localEulerAngles = _transform.localEulerAngles;
            localEulerAngles.z = angle;
            _transform.localEulerAngles = localEulerAngles;

            _iconImageTransform.rotation = _iconRotationDefault;
        }
        
        [Serializable]
        private struct PresetData
        {
            public Sprite IconImageSprite;
            public Color Color;
        }
    }
}