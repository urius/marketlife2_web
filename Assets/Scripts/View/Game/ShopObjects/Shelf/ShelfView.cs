using System;
using Other;
using UnityEngine;

namespace View.Game.ShopObjects.Shelf
{
    [SelectionBase]
    public class ShelfView : MonoBehaviour
    {
        [Serializable]
        private class ProductPlaceholder
        {
            public Transform Transform;
            public int SortingOrder;
        }
        
        [LabeledArray(nameof(ProductPlaceholder.Transform))]
        [SerializeField] private ProductPlaceholder[] _productPlaceholders;


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