using Cysharp.Threading.Tasks;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model.People;
using Model.People.States;
using Model.People.States.Customer;
using UnityEngine;
using Utils;
using View.Game.Product;
using View.Helpers;

namespace View.Game.People
{
    public class CustomerCharMediator : BotCharMediatorBase<CustomerCharModel>
    {
        private const float PutProductDuration = 0.5f;
        
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        
        private readonly TakeProductContext _takeProductContext = new ();
        
        private ManView _customerView;
        private int _flyingProductFromBasketAnimationIndex = 0;

        protected override void MediateInternal()
        {
            base.MediateInternal();
            
            _customerView = InstantiatePrefab<ManView>(PrefabKey.Man);
            _customerView.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);
            SetClothes();

            Subscribe();
            
            _eventBus.Dispatch(new CustomerInitializedEvent(TargetModel));
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_customerView);
            _customerView = null;
            
            base.UnmediateInternal();
        }

        protected override ManView View => _customerView;
        protected override void ToWalkingState()
        {
            _customerView.ToWalkState(TargetModel.HasProducts);
        }
        
        protected override void ToIdleState()
        {
            _customerView.ToIdleState(TargetModel.HasProducts);
        }

        protected override void StepFinishedHandler()
        {
            _eventBus.Dispatch(new CustomerStepFinishedEvent(TargetModel));
        }

        private void Subscribe()
        {
            TargetModel.StateChanged += OnStateChanged;
            TargetModel.ProductAdded += OnProductAdded;
        }

        private void Unsubscribe()
        {
            TargetModel.StateChanged -= OnStateChanged;
            TargetModel.ProductAdded -= OnProductAdded;
        }

        private void OnStateChanged(ShopSharStateBase state)
        {
            if (state.StateName == ShopCharStateName.TakingProduct)
            {
                var takeProductState = (CustomerTakeProductFromShelfState)state;
                AnimateTakingProductFromShelf(takeProductState);
            }
            else if (state.StateName == ShopCharStateName.Paying)
            {
                AnimatePaying();
            }
        }

        private void SetClothes()
        {
            var clothes = ManSpriteTypesHelper.GetCustomerRandomClothes();
            var hairType = ManSpriteTypesHelper.GetRandomHair();
            var bodySprite = _spritesHolderSo.GetManSpriteByKey(clothes.BodyClothes);
            var handSprite = _spritesHolderSo.GetManSpriteByKey(clothes.HandClothes);
            var footSprite = _spritesHolderSo.GetManSpriteByKey(clothes.FootClothes);
            var hairSprite = _spritesHolderSo.GetManSpriteByKey(hairType);

            _customerView.SetClothesSprites(bodySprite, handSprite, footSprite);
            _customerView.SetHairSprite(hairSprite);

            if (Random.value < 0.8)
            {
                _customerView.SetGlassesSprite(null);
            }
            else
            {
                var glassesType = ManSpriteTypesHelper.GetRandomGlasses();
                var glassesSprite = _spritesHolderSo.GetManSpriteByKey(glassesType);
                _customerView.SetGlassesSprite(glassesSprite);
            }

            if (DateTimeHelper.IsNewYearsEve())
            {
                var hatSprite = _spritesHolderSo.GetManSpriteByKey(ManSpriteType.SantaHat);
                _customerView.SetHatSprite(hatSprite);
            }
            else
            {
                _customerView.SetHatSprite(null);
            }
        }

        private void AnimatePaying()
        {
            DisableSwitchToIdleOnNextFrame();

            _customerView.ToLeftSide();
            _customerView.ToTakingProductState();

            _flyingProductFromBasketAnimationIndex = 0;
            AnimateProductFlyingFromBasket(_flyingProductFromBasketAnimationIndex);
        }

        private void AnimateProductFlyingFromBasket(int slotIndex)
        {
            var productView = GetFromCache<ProductView>(PrefabKey.ProductView);
            var productType = TargetModel.GetProductTypeAtSlot(slotIndex);
            var productSprite = _spritesHolderSo.GetProductSpriteByKey(productType);
            productView.SetSprite(productSprite);
            var startPosition = _customerView.GetProductInBasketPosition(slotIndex);
            var productTransform = productView.transform;
            productTransform.position = startPosition;
            
            _customerView.SetProductInBasketSprite(slotIndex, null);
            
            productTransform
                .LeanMove(startPosition + new Vector3(0, 0, 1), 0.5f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnUpdate(OnAnimateProductFlyingFromBasketUpdate)
                .setOnUpdateParam(productView)
                .setOnComplete(OnAnimateProductFlyingFromBasketComplete)
                .setOnCompleteParam(productView);
        }

        private void OnAnimateProductFlyingFromBasketUpdate(float progress, object productView)
        {
            ((ProductView)productView).SetAlpha(1 - progress);
        }

        private void OnAnimateProductFlyingFromBasketComplete(object productView)
        {
            ReturnToCache(((ProductView)productView).gameObject);

            _flyingProductFromBasketAnimationIndex++;
            
            if (_flyingProductFromBasketAnimationIndex < TargetModel.ProductsCount)
            {
                AnimateProductFlyingFromBasket(_flyingProductFromBasketAnimationIndex);
            }
            else
            {
                _customerView.SetProductsBasketVisibility(false);

                DispatchAnimationFinishedWithDelay();
            }
        }

        private async void DispatchAnimationFinishedWithDelay()
        {
            await UniTask.Delay(500);
            
            _eventBus.Dispatch(new CustomerFlyProductFromBasketAnimationFinishedEvent(TargetModel));
        }

        private void AnimateTakingProductFromShelf(CustomerTakeProductFromShelfState takeProductState)
        {
            DisableSwitchToIdleOnNextFrame();
            
            _customerView.ToRightSide();
            
            var slotPositionProvider = _sharedViewsDataHolder.GetShelfSlotPositionProvider(takeProductState.TargetShelfModel);
            var slotPosition = slotPositionProvider.GetSlotWorldPosition(takeProductState.SlotIndex);
            var basketSlotIndex = takeProductState.BasketSlotIndex;
            var targetPosition = _customerView.GetProductInBasketPosition(basketSlotIndex);
            var productView = CreteProductViewForTakeAnimation(takeProductState.ProductType, slotPosition);

            _takeProductContext.Set(basketSlotIndex, productView);

            _customerView.SetProductInBasketSprite(basketSlotIndex, null);
            _customerView.ToTakingProductState();

            productView.transform.LeanMove(targetPosition, PutProductDuration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(OnAnimateTakeProductComplete);
        }

        private ProductView CreteProductViewForTakeAnimation(ProductType productType,
            Vector3 slotPosition)
        {
            var productView = GetFromCache<ProductView>(PrefabKey.ProductView);
            var productSprite = _spritesHolderSo.GetProductSpriteByKey(productType);
            productView.SetSortingLayerName(Constants.GeneralTopSortingLayerName);
            productView.SetSprite(productSprite);
            productView.transform.position = slotPosition;
            
            return productView;
        }

        private void OnAnimateTakeProductComplete()
        {
            ReturnToCache(_takeProductContext.ProductView.gameObject);
            DisplayBasketSlot(_takeProductContext.BasketSlotIndex);
            
            _takeProductContext.Reset();
            
            _eventBus.Dispatch(new CustomerTakeProductAnimationFinishedEvent(TargetModel));
        }

        private void OnProductAdded(int slotIndex)
        {
            DisplayBasketSlot(slotIndex);
            _customerView.SetProductsBasketVisibility(TargetModel.HasProducts);
        }

        private void DisplayBasketSlot(int slotIndex)
        {
            var productType = TargetModel.GetProductTypeAtSlot(slotIndex);

            var sprite = productType != ProductType.None ? _spritesHolderSo.GetProductSpriteByKey(productType) : null;

            _customerView.SetProductInBasketSprite(slotIndex, sprite);
        }
        
        private class TakeProductContext
        {
            public int BasketSlotIndex;
            public ProductView ProductView;

            public void Set(int basketSlotIndex, ProductView productView)
            {
                BasketSlotIndex = basketSlotIndex;
                ProductView = productView;
            }

            public void Reset()
            {
                BasketSlotIndex = default;
                ProductView = default;
            }
        }
    }
}