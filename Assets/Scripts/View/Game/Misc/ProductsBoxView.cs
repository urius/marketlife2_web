using Other;
using UnityEngine;
using View.Game.Shared;

namespace View.Game.Misc
{
    public class ProductsBoxView : MonoBehaviour
    {
        [SerializeField]
        [LabeledArray(nameof(ProductPlaceholder.Transform))]
        private ProductPlaceholder[] _productPlaceholders;
        
        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            if (_productPlaceholders == null) return;
            
            foreach (var productPlaceholder in _productPlaceholders)
            {
                Gizmos.DrawSphere(productPlaceholder.Transform.position, 0.1f);
            }
        }
    }
}