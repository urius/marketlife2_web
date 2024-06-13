using System;
using Data;
using Other;
using UnityEngine;
using View.Game.Product;
using View.Game.Shared;
using View.Game.ShopObjects.Common;

namespace View.Game.ShopObjects.Shelf
{
    [SelectionBase]
    public class ShelfView : ShopObjectViewBase
    {
        [SerializeField] 
        private ShopObjectType _shelfType;
        
        [SerializeField] 
        private int _shelfUpgradeIndex;

        [SerializeField]
        [LabeledArray(nameof(ProductPlaceholder.Transform))]
        private ProductPlaceholder[] _productPlaceholders;

        [SerializeField]
        private ProductView[] _productViews;
        
        public ShopObjectType ShelfType => _shelfType;
        public int ShelfUpgradeIndex => _shelfUpgradeIndex;
        public int SlotsAmount => _productViews.Length;
        
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