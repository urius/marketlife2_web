using TMPro;
using UnityEngine;

namespace View.Game.BuildPoint
{
    public class BuildPointView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private SpriteRenderer _iconOnTooltipRenderer;
        [SerializeField] private SpriteRenderer _iconOnSquareRenderer;
        [SerializeField] private SpriteRenderer _squareSpriteRenderer;
        [SerializeField] private Color _squareActiveColor;
        [SerializeField] private Color _squareLockedColor;

        private void OnDisable()
        {
            SetIconOnSquareSprite(null);
            SetSquareActiveColor();
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

        public void SetSquareActiveColor()
        {
            _squareSpriteRenderer.color = _squareActiveColor;
        }
        
        public void SetSquareLockedColor()
        {
            _squareSpriteRenderer.color = _squareLockedColor;
        }
    }
}