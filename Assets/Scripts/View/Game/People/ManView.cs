using System;
using System.Linq;
using Other;
using UnityEngine;
using UnityEngine.Rendering;
using View.Game.Misc;
using View.Game.Shared;

namespace View.Game.People
{
    public class ManView : MonoBehaviour, ISortableView, IPlayerCharPositionsProvider, ICharProductsInBoxPositionsProvider
    {
        [SerializeField] private SortingGroup _sortingGroup;
        [SerializeField] private Animation _animation;
        [SerializeField, LabeledArray(nameof(AnimationData.AnimationKey))] private AnimationData[] _animationsData;
        [SerializeField] private Transform _productsBoxPlaceholderTransform;
        [SerializeField] private GameObject _productsBoxPrefab;
        [SerializeField] private GameObject _productsBasketPrefab;
        [SerializeField] private SpriteRenderer _hand2SpriteRenderer;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private SpriteRenderer _hairSpriteRenderer;
        [SerializeField] private SpriteRenderer _bodyClothesSpriteRenderer;
        [SerializeField] private SpriteRenderer _hand1ClothesSpriteRenderer;
        [SerializeField] private SpriteRenderer _hand2ClothesSpriteRenderer;
        [SerializeField] private SpriteRenderer _leg1ClothesSpriteRenderer;
        [SerializeField] private SpriteRenderer _leg2ClothesSpriteRenderer;
        [SerializeField] private SpriteRenderer _hatSpriteRenderer;
        [SerializeField] private SpriteRenderer _glassesSpriteRenderer;
        
        private AnimationKey _currentAnimationKey = AnimationKey.None;
        private ProductsBoxView _productsBoxView;
        private ProductsBasketView _productsBasketView;

        public Transform ProductsBoxPlaceholderTransform => _productsBoxPlaceholderTransform;
        public Transform CenterPointTransform => _bodyTransform;

        public Vector3 GetProductInBoxPosition(int productIndex)
        {
            if (productIndex < _productsBoxView.ProductViews.Length)
            {
                return _productsBoxView.ProductViews[productIndex].transform.position;
            }

            return _productsBoxView != null ? _productsBoxView.transform.position : Vector3.zero;
        }
        
        public Vector3 GetProductInBasketPosition(int productIndex)
        {
            if (productIndex < _productsBasketView.ProductViews.Length)
            {
                return _productsBasketView.ProductViews[productIndex].transform.position;
            }

            return _productsBasketView != null ? _productsBasketView.transform.position : Vector3.zero;
        }

        public void SetProductsBoxVisibility(bool isVisible)
        {
            if (isVisible)
            {
                CreateProductsBoxViewIfNeeded();
            }

            if (_productsBoxView != null)
            {
                _productsBoxPlaceholderTransform.gameObject.SetActive(isVisible);
                _productsBoxView.SetVisible(isVisible);
            }
        }
        
        public void SetProductsBasketVisibility(bool isVisible)
        {
            if (isVisible)
            {
                CreateProductsBasketViewIfNeeded();
            }

            if (_productsBasketView != null)
            {
                _productsBoxPlaceholderTransform.gameObject.SetActive(isVisible);
                _productsBasketView.SetVisible(isVisible);
            }
        }

        public void SetProductInBoxSprite(int productIndex, Sprite sprite)
        {
            CreateProductsBoxViewIfNeeded();
            
            _productsBoxView.SetProductSprite(productIndex, sprite);
        }
        
        public void SetProductInBasketSprite(int productIndex, Sprite sprite)
        {
            CreateProductsBasketViewIfNeeded();
    
            _productsBasketView.SetProductSprite(productIndex, sprite);
        }

        public void SetSortingOrder(int order)
        {
            _sortingGroup.sortingOrder = order;
        }
        
        public void SetSortingLayerName(string sortingLayerName)
        {
            _sortingGroup.sortingLayerName = sortingLayerName;
        }

        public void ToRightSide()
        {
            var transform1 = transform;
            
            var scale = transform1.localScale;
            scale.x = scale.z = -1;
            transform1.localScale = scale;
        }

        public void ToLeftSide()
        {
            var transform1 = transform;
            
            var scale = transform1.localScale;
            scale.x = scale.z = 1;
            transform1.localScale = scale;
        }

        public void ToIdleState(bool withPayloadInHands = false)
        {
            PlayAnimationByKey(withPayloadInHands ? AnimationKey.IdleWithBox : AnimationKey.Idle);
        }

        public void ToWalkState(bool withPayloadInHands = false)
        {
            PlayAnimationByKey(withPayloadInHands ? AnimationKey.WalkWithBox : AnimationKey.Walk);
        }

        public void ToTakingProductState()
        {
            PlayAnimationByKey(AnimationKey.TakingProduct);
        }
        
        public void ToPayingState()
        {
            PlayAnimationByKey(AnimationKey.TakingProduct);
        }

        public void ToAcceptingPayState()
        {
            PlayAnimationByKey(AnimationKey.TakingProduct);
        }

        public void SetClothesSprites(Sprite bodySprite, Sprite handSprite, Sprite footSprite)
        {
            _bodyClothesSpriteRenderer.sprite = bodySprite;
            _hand1ClothesSpriteRenderer.sprite = handSprite;
            _hand2ClothesSpriteRenderer.sprite = handSprite;
            _leg1ClothesSpriteRenderer.sprite = footSprite;
            _leg2ClothesSpriteRenderer.sprite = footSprite; 
        }

        public void SetHatSprite(Sprite hatSprite)
        {
            _hatSpriteRenderer.sprite = hatSprite;
        }

        public void SetGlassesSprite(Sprite glassesSprite)
        {
            _glassesSpriteRenderer.sprite = glassesSprite;
        }

        public void SetHairSprite(Sprite hairSprite)
        {
            _hairSpriteRenderer.sprite = hairSprite;
        }

        private void CreateProductsBoxViewIfNeeded()
        {
            if (_productsBoxView == null)
            {
                var go = Instantiate(_productsBoxPrefab, _productsBoxPlaceholderTransform);
                _productsBoxView = go.GetComponent<ProductsBoxView>();
                
                _productsBoxView.SetSortingLayerId(_hand2SpriteRenderer.sortingLayerID);
                _productsBoxView.SetSortingOrder(_hand2SpriteRenderer.sortingOrder - 1);
            }
        }
        
        private void CreateProductsBasketViewIfNeeded()
        {
            if (_productsBasketView == null)
            {
                var go = Instantiate(_productsBasketPrefab, _productsBoxPlaceholderTransform);
                _productsBasketView = go.GetComponent<ProductsBasketView>();
                
                _productsBasketView.SetSortingLayerId(_hand2SpriteRenderer.sortingLayerID);
                _productsBasketView.SetSortingOrder(_hand2SpriteRenderer.sortingOrder - 1);
            }
        }

        private void PlayAnimationByKey(AnimationKey animationKey)
        {
            if (_currentAnimationKey == animationKey) return;
            
            _currentAnimationKey = animationKey;

            if (_animationsData.Any(a => a.AnimationKey == animationKey))
            {
                var animationData = _animationsData.FirstOrDefault(a => a.AnimationKey == animationKey);

                //_animation.Stop();
                //_animation.clip = animationData.Animation;
                _animation.Play(animationData.Animation.name);
            }
        }

        [Serializable]
        private struct AnimationData
        {
            public AnimationKey AnimationKey;
            public AnimationClip Animation;
        }

        private enum AnimationKey
        {
            None,
            Idle,
            Walk,
            WalkWithBox,
            IdleWithBox,
            TakingProduct,
        }
    }
}