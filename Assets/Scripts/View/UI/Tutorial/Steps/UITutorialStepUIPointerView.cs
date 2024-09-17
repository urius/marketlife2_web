using TMPro;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepUIPointerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _pointerRectTransform;
        [SerializeField] private RectTransform _arrowRectTransform;
        
        public void ToLeftSideState()
        {
            SetTextXPivot(1);
            SetArrowZAngle(190);
        }

        public void ToRightSideState()
        {
            SetTextXPivot(0);
            SetArrowZAngle(170);
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

        public void SetText(string newText)
        {
            _text.text = newText;
        }

        public void SetPointerToPosition(Vector3 worldPosition)
        {
            var mainCamera = UnityEngine.Camera.main;
            
            var screenPoint = mainCamera.WorldToScreenPoint(worldPosition);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_pointerRectTransform.parent as RectTransform, screenPoint, mainCamera, out var localPosition);
            
            _pointerRectTransform.anchoredPosition = localPosition;
        }
    }
}