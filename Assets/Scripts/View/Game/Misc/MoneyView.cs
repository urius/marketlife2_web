using UnityEngine;

namespace View.Game.Misc
{
    public class MoneyView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private float _defaultAlpha;

        private void Awake()
        {
            _defaultAlpha = _spriteRenderer.color.a;
        }

        private void OnEnable()
        {
            SetSortingOrder(1);
            SetAlpha(_defaultAlpha);
        }

        public void SetSortingOrder(int sortingOrder)
        {
            _spriteRenderer.sortingOrder = sortingOrder;
        }

        public void SetAlpha(float alpha)
        {
            var color = _spriteRenderer.color;
            color.a = alpha;

            _spriteRenderer.color = color;
        }
    }
}