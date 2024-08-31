using TMPro;
using UnityEngine;

namespace View.Game.BuildPoint
{
    public class BuildPointView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private SpriteRenderer _iconOnTooltipRenderer;
        [SerializeField] private SpriteRenderer _iconOnSquareRenderer;

        private void OnDisable()
        {
            SetIconOnSquareSprite(null);
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetIconOnSquareSprite(Sprite sprite)
        {
            _iconOnSquareRenderer.sprite = sprite;
        }

        public void SetIconOnTooltipSprite(Sprite sprite)
        {
            _iconOnTooltipRenderer.sprite = sprite;
        }
    }
}