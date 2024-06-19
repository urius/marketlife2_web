using System;
using System.Linq;
using Other;
using UnityEngine;
using UnityEngine.Rendering;
using View.Game.Misc;
using View.Game.Shared;

namespace View.Game.People
{
    public class ManView : MonoBehaviour, ISortableView
    {
        [SerializeField] private SortingGroup _sortingGroup;
        [SerializeField] private Animation _animation;
        [SerializeField, LabeledArray(nameof(AnimationData.AnimationKey))] private AnimationData[] _animationsData;
        [SerializeField] private Transform _productsBoxPlaceholderTransform;
        [SerializeField] private GameObject _productsBoxPrefab;
        [SerializeField] private SpriteRenderer _hand2SpriteRenderer;
        
        private AnimationKey _currentAnimationKey = AnimationKey.None;
        private ProductsBoxView _productsBoxView;

        public Transform ProductsBoxPlaceholderTransform => _productsBoxPlaceholderTransform;
        
        public ProductsBoxView GetProductBoxView()
        {
            return _productsBoxView;
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

        public void SetProductSprite(int productIndex, Sprite sprite)
        {
            CreateProductsBoxViewIfNeeded();
            
            _productsBoxView.SetProductSprite(productIndex, sprite);
        }

        public void SetSortingOrder(int order)
        {
            _sortingGroup.sortingOrder = order;
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

        private void PlayAnimationByKey(AnimationKey animationKey)
        {
            if (_currentAnimationKey == animationKey) return;
            
            _currentAnimationKey = animationKey;
            
            if (_animationsData.Any(a => a.AnimationKey == animationKey))
            {
                var animationData = _animationsData.FirstOrDefault(a => a.AnimationKey == animationKey);

                _animation.clip = animationData.Animation;
                _animation.Play();
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
        }
    }
}