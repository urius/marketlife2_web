using UnityEngine;

namespace View.Game.Product
{
    public class ProductView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

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
    }
}