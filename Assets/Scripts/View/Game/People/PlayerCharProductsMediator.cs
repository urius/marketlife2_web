using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using UnityEngine;
using View.Game.Misc;

namespace View.Game.People
{
    public class PlayerCharProductsMediator : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly ISharedViewsDataHolder _viewsDataHolder = Instance.Get<ISharedViewsDataHolder>();

        private readonly ManView _playerCharView;
        private readonly PlayerCharModel _playerCharModel;
        
        private TakeBoxAnimationContext _takeBoxAnimationContext;

        public PlayerCharProductsMediator(ManView playerCharView)
        {
            _playerCharView = playerCharView;
            _playerCharModel = _playerModelHolder.PlayerModel.PlayerCharModel;
        }

        protected override void MediateInternal()
        {
            _viewsDataHolder.RegisterPlayerCharBoxProductsPositionProvider(_playerCharView);
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            _viewsDataHolder.UnregisterPlayerCharBoxPositionProvider();
            
            Unsubscribe();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<AnimateTakeBoxFromTruckEvent>(AnimateTakeBoxFromTruckEventHandler);
            _playerCharModel.ProductsBoxAdded += OnProductsBoxAdded;
            _playerCharModel.ProductRemoved += OnProductRemoved;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<AnimateTakeBoxFromTruckEvent>(AnimateTakeBoxFromTruckEventHandler);
            _playerCharModel.ProductsBoxAdded -= OnProductsBoxAdded;
            _playerCharModel.ProductRemoved -= OnProductRemoved;

            _updatesProvider.FixedUpdateHappened -= OnTakeBoxAnimationFixedUpdate;
        }

        private void OnProductsBoxAdded()
        {
            UpdateProductBoxView();
        }

        private void OnProductRemoved(int slotIndex)
        {
            _playerCharView.SetProductsBoxVisibility(_playerCharModel.HasProducts);
            DisplayProduct(slotIndex);
        }

        private void UpdateProductBoxView()
        {
            var hasProducts = _playerCharModel.HasProducts;
            _playerCharView.SetProductsBoxVisibility(hasProducts);

            if (hasProducts)
            {
                for (var i = 0; i < _playerCharModel.ProductsInBox.Count; i++)
                {
                    DisplayProduct(i);
                }
            }
        }

        private void DisplayProduct(int slotIndex)
        {
            var sprite = _spritesHolderSo.GetProductSpriteByKey(_playerCharModel.ProductsInBox[slotIndex]);
            _playerCharView.SetProductSprite(slotIndex, sprite);
        }

        private void AnimateTakeBoxFromTruckEventHandler(AnimateTakeBoxFromTruckEvent e)
        {
            var startPosition = _sharedViewsDataHolder
                .GetTruckBoxPositionsProvider(e.TruckPointModel)
                .GetBoxWorldPosition(e.ProductBoxIndexToTake);

            var animatedBoxView = InstantiatePrefab<ProductsBoxView>(PrefabKey.ProductsBox);
            var productSprite = _spritesHolderSo.GetProductSpriteByKey(_playerCharModel.ProductsInBox[0]);
            animatedBoxView.SetProductsSprite(productSprite);
            
            var transform = animatedBoxView.transform;
            transform.localEulerAngles = new Vector3(145, 45, 60);
            transform.position = startPosition;
            animatedBoxView.SetTopSortingLayer();

            _takeBoxAnimationContext = new TakeBoxAnimationContext
            {
                StartPosition = startPosition,
                TargetTransform = _playerCharView.ProductsBoxPlaceholderTransform,
                BoxView = animatedBoxView,
                Speed = 5,
            };
            
            _playerCharView.SetProductsBoxVisibility(false);

            _updatesProvider.FixedUpdateHappened -= OnTakeBoxAnimationFixedUpdate;
            _updatesProvider.FixedUpdateHappened += OnTakeBoxAnimationFixedUpdate;
        }

        private void OnTakeBoxAnimationFixedUpdate()
        {
            if (_takeBoxAnimationContext == null)
            {
                _updatesProvider.FixedUpdateHappened -= OnTakeBoxAnimationFixedUpdate;
                return;
            }

            _takeBoxAnimationContext.Progress += Time.deltaTime * _takeBoxAnimationContext.Speed;

            _takeBoxAnimationContext.BoxView.transform.position = Vector3.Slerp(
                _takeBoxAnimationContext.StartPosition,
                _takeBoxAnimationContext.TargetTransform.position,
                _takeBoxAnimationContext.Progress);

            if (_takeBoxAnimationContext.Progress >= 1)
            {
                Destroy(_takeBoxAnimationContext.BoxView);
                _takeBoxAnimationContext = null;
                
                _playerCharView.SetProductsBoxVisibility(true);
            }
        }

        private class TakeBoxAnimationContext
        {
            public Vector3 StartPosition;
            public ProductsBoxView BoxView;
            public Transform TargetTransform;
            public float Progress = 0;
            public float Speed = 1;
        }
    }
}