using System;
using TMPro;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepUIPointerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _pointerRectTransform;
        [SerializeField] private RectTransform _arrowRectTransform;
        
        public void ToBottomLeftSideState()
        {
            SetTextXPivot(1);
            SetTextYPivot(0);
            
            SetTextYPosAbsMultiplier(1);
            SetArrowZAngle(190);
        }

        public void ToBottomRightSideState()
        {
            SetTextXPivot(0);
            SetTextYPivot(0);
            
            SetTextYPosAbsMultiplier(1);
            SetArrowZAngle(170);
        }

        public void ToTopLeftSideState()
        {
            SetTextXPivot(1);
            SetTextYPivot(1);

            SetTextYPosAbsMultiplier(-1);
            SetArrowZAngle(10);
        }

        private void SetTextYPosAbsMultiplier(int multiplier)
        {
            var textPos = ((RectTransform)_text.transform).anchoredPosition;
            ((RectTransform)_text.transform).anchoredPosition =
                new Vector2(textPos.x, multiplier * Math.Abs(textPos.y));
        }

        private void SetArrowZAngle(float angle)
        {
            var localEulerAngles = _arrowRectTransform.localEulerAngles;
            localEulerAngles.z = angle;
            _arrowRectTransform.localEulerAngles = localEulerAngles;
        }

        private void SetTextXPivot(float xPivot)
        {
            var textRectTransform = _text.transform as RectTransform;
            textRectTransform.pivot = new Vector2(xPivot, textRectTransform.pivot.y);
        }

        private void SetTextYPivot(float yPivot)
        {
            var textRectTransform = _text.transform as RectTransform;
            textRectTransform.pivot = new Vector2(textRectTransform.pivot.x, yPivot);
        }

        public void SetText(string newText)
        {
            _text.text = newText;
        }

        public void SetPointerToPosition(Vector3 worldPosition, Vector2 anchoredOffset = default)
        {
            var mainCamera = UnityEngine.Camera.main;
            
            var screenPoint = mainCamera.WorldToScreenPoint(worldPosition);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_pointerRectTransform.parent as RectTransform, screenPoint, mainCamera, out var localPosition);
            
            _pointerRectTransform.anchoredPosition = localPosition + anchoredOffset;
        }
    }
}