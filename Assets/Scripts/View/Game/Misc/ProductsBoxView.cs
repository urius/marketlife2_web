using Other;
using UnityEngine;
using View.Game.Product;
using View.Game.Shared;

namespace View.Game.Misc
{
    public class ProductsBoxView : MonoBehaviour
    {
        [SerializeField]
        [LabeledArray(nameof(ProductPlaceholder.Transform))]
        private ProductView[] _products;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetProductsSprite(Sprite sprite)
        {
            foreach (var productView in _products)
            {
                productView.SetSprite(sprite);
            }
        }
        
        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            if (_products == null) return;
            
            foreach (var productPlaceholder in _products)
            {
                //Gizmos.DrawSphere(productPlaceholder.Transform.position, 0.1f);
            }
        }
    }
}