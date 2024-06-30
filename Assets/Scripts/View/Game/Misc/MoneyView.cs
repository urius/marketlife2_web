using UnityEngine;

namespace View.Game.Misc
{
    public class MoneyView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private void OnEnable()
        {
            SetSortingOrder(1);
        }

        public void SetSortingOrder(int sortingOrder)
        {
            _spriteRenderer.sortingOrder = sortingOrder;
        }
    }
}