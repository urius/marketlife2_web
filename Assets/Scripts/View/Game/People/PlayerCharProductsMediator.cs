using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using View.Game.Misc;
using View.Game.Shared;
using View.Helpers;

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
            _viewsDataHolder.RegisterPlayerCharPositionProvider(_playerCharView);
            _viewsDataHolder.RegisterCharProductsInBoxPositionsProvider(_playerCharModel.ProductsBox, _playerCharView);
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            _viewsDataHolder.UnregisterPlayerCharPositionProvider();
            _viewsDataHolder.UnregisterCharProductsInBoxPositionsProvider(_playerCharModel.ProductsBox);
            
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

            _updatesProvider.GameplayFixedUpdate -= OnTakeBoxAnimationGameplayFixedUpdate;
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
            _playerCharView.SetProductInBoxSprite(slotIndex, sprite);
        }

        private void AnimateTakeBoxFromTruckEventHandler(AnimateTakeBoxFromTruckEvent e)
        {
            var startPosition = _sharedViewsDataHolder
                .GetTruckPointBoxPositions(e.TruckPointModel, e.ProductBoxIndexToTake);

            var animatedBoxView = InstantiatePrefab<ProductsBoxView>(PrefabKey.ProductsBox);
            var productType = _playerCharModel.ProductsInBox[0];
            
            _takeBoxAnimationContext = AnimationHelper.InitTakeBoxFromTruckPointAnimation(_playerCharView, animatedBoxView, productType, startPosition);

            _updatesProvider.GameplayFixedUpdate -= OnTakeBoxAnimationGameplayFixedUpdate;
            _updatesProvider.GameplayFixedUpdate += OnTakeBoxAnimationGameplayFixedUpdate;
        }

        private void OnTakeBoxAnimationGameplayFixedUpdate()
        {
            if (_takeBoxAnimationContext == null)
            {
                _updatesProvider.GameplayFixedUpdate -= OnTakeBoxAnimationGameplayFixedUpdate;
                return;
            }

            var progress = AnimationHelper.TakeBoxAnimationUpdate(_takeBoxAnimationContext);

            if (progress >= 1)
            {
                Destroy(_takeBoxAnimationContext.BoxView);
                _takeBoxAnimationContext = null;
            }
        }
    }
}