using UnityEngine;

namespace View.Game.Product
{
    public class ProductView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private void OnDisable()
        {
            SetAlpha(1);
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void SetSortingOrder(int order)
        {
            _spriteRenderer.sortingOrder = order;
        }
        
        public void SetSortingLayerName(string sortingLayerName)
        {
            _spriteRenderer.sortingLayerName = sortingLayerName;
        }

        public void SetAlpha(float alpha)
        {
            var color = _spriteRenderer.color;
            color.a = alpha;
            _spriteRenderer.color = color;
        }
    }
}