using TMPro;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepUIPointerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _pointerRectTransform;

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