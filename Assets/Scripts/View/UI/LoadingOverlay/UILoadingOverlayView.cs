using TMPro;
using UnityEngine;

namespace View.UI.LoadingOverlay
{
    public class UILoadingOverlayView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _messageText;

        public float Alpha => _canvasGroup.alpha;
        
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void SetMessageTextVisibility(bool isVisible)
        {
            _messageText.gameObject.SetActive(isVisible);
        }

        public void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        public void SetMessageText(string text)
        {
            _messageText.text = text;
        }
    }
}