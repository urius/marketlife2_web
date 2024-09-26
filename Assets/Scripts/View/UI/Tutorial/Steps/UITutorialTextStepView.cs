using TMPro;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialTextStepView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private float _updateTimer;

        public void SetVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
        
        public void SetText(string text)
        {
            _text.text = text;
        }

        private void FixedUpdate()
        {
            _updateTimer += Time.fixedDeltaTime;
            var pingPongValue = Mathf.PingPong(_updateTimer, 1f); // PingPong between 0 and 1
            var alpha = Mathf.Lerp(0.75f, 1f, pingPongValue); // Lerp between 0.5 and 1 based on lerp value
            
            SetTextAlpha(alpha);
        }

        private void SetTextAlpha(float alpha)
        {
            var color = _text.color;
            color.a = alpha;
            _text.color = color;
        }
    }
}