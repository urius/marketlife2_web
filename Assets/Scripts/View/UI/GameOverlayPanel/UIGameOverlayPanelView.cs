namespace View.UI.GameOverlayPanel
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIGameOverlayPanelView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public delegate void PointerEvent(PointerEventData eventData);

        public event PointerEvent OnPointerDownEvent;
        public event PointerEvent OnPointerUpEvent;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerDownEvent?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Invoke the OnPointerUp event
            OnPointerUpEvent?.Invoke(eventData);
        }
    }
}