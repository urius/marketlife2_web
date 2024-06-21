using Data;
using UnityEngine;
using View.Game.Product;
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
        private ProductView[] _productViews;
        
        public ShopObjectType ShelfType => _shelfType;
        public int ShelfUpgradeIndex => _shelfUpgradeIndex;
        public int SlotsAmount => _productViews.Length;

        public Vector3 GetSlotPosition(int productIndex)
        {
            return productIndex < _productViews.Length ? _productViews[productIndex].transform.position : Vector3.zero;
        }

        public void SetProductSprite(int productIndex, Sprite sprite)
        {
            if (productIndex < _productViews.Length)
            {
                _productViews[productIndex].SetSprite(sprite);
            }
        }
    }
}