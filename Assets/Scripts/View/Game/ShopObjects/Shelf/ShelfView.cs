using System;
using Data;
using Other;
using UnityEngine;
using View.Game.ShopObjects.Common;

namespace View.Game.ShopObjects.Shelf
{
    [SelectionBase]
    public class ShelfView : ShopObjectViewBase
    {
        [Serializable]
        private class ProductPlaceholder
        {
            public Transform Transform;
            public int SortingOrder;
        }
        
        [SerializeField] 
        private ShopObjectType _shelfType;
        
        [SerializeField] 
        private int _shelfUpgradeIndex;

        [SerializeField]
        [LabeledArray(nameof(ProductPlaceholder.Transform))]
        private ProductPlaceholder[] _productPlaceholders;

        public ShopObjectType ShelfType => _shelfType;
        public int ShelfUpgradeIndex => _shelfUpgradeIndex;
        public int SlotsAmount => _productPlaceholders.Length;
        
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