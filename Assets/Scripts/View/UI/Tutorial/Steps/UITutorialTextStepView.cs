using TMPro;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialTextStepView : MonoBehaviour
    {
        //private const float TextAlphaDuration = 1f; // Duration for the alpha transition
        
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _arrowRectTransform;

        private float _updateTimer;

        public void SetVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
        
        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetArrowAngle(float angle)
        {
            var localEulerAngles = _arrowRectTransform.localEulerAngles;
            localEulerAngles.z = angle;
            _arrowRectTransform.localEulerAngles = localEulerAngles;
        }

        public void SetArrowVisibility(bool isVisible)
        {
            _arrowRectTransform.gameObject.SetActive(isVisible);
        }

        // private void FixedUpdate()
        // {
        //     _updateTimer += Time.fixedDeltaTime;
        //     var pingPongValue = Mathf.PingPong(_updateTimer, 1f); // PingPong between 0 and 1
        //     var alpha = Mathf.Lerp(0.75f, 1f, pingPongValue); // Lerp between 0.5 and 1 based on lerp value
        //     
        //     SetTextAlpha(alpha);
        // }

        private void SetTextAlpha(float alpha)
        {
            var color = _text.color;
            color.a = alpha;
            _text.color = color;
        }
    }
}