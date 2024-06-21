using System.Runtime.CompilerServices;
using Data;
using Other;
using UnityEngine;
using UnityEngine.Rendering;
using View.Game.Product;
using View.Game.Shared;

namespace View.Game.Misc
{
    public class ProductsBoxView : MonoBehaviour
    {
        [SerializeField]
        [LabeledArray(nameof(ProductPlaceholder.Transform))]
        private ProductView[] _products;
        
        [SerializeField]
        private SortingGroup _sortingGroup;

        public ProductView[] ProductViews => _products;

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
        
        public void SetProductSprite(int index, Sprite sprite)
        {
            if (index < _products.Length)
            {
                _products[index].SetSprite(sprite);
            }
        }
        
        public void SetSortingOrder(int order)
        {
            _sortingGroup.sortingOrder = order;
        }
        
        public void SetSortingLayerId(int sortingLayerId)
        {
            _sortingGroup.sortingLayerID = sortingLayerId;
        }
        
        public void SetTopSortingLayer()
        {
            _sortingGroup.sortingLayerName = Constants.GeneralTopSortingLayerName;
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