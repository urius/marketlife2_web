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
using View.Game.Shared;
using View.Helpers;

namespace View.Game.People
{
    public class CustomerCharMediator : MediatorWithModelBase<CustomerCharModel>
    {
        private const int Speed = 2;
        private const float PutProductDuration = 0.5f;
        
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        
        private readonly WalkContext _walkContext = new ();
        private readonly TakeProductContext _takeProductContext = new ();
        
        private ManView _customerView;
        private bool _idleStateRequestFlag = false;
        private int _flyingProductFromBasketAnimationIndex = 0;

        protected override void MediateInternal()
        {
            _customerView = InstantiatePrefab<ManView>(PrefabKey.Man);
            
            _customerView.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellPosition);

            Subscribe();

            SetClothes();
            
            _eventBus.Dispatch(new CustomerInitializedEvent(TargetModel));
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_customerView);
            _customerView = null;
        }

        private void Subscribe()
        {
            TargetModel.CellPositionChanged += OnCellPositionChanged;
            TargetModel.StateChanged += OnStateChanged;
            TargetModel.ProductAdded += OnProductAdded;
        }

        private void Unsubscribe()
        {
            TargetModel.CellPositionChanged -= OnCellPositionChanged;
            TargetModel.StateChanged -= OnStateChanged;
            TargetModel.ProductAdded -= OnProductAdded;
            
            _updatesProvider.GameplayFixedUpdate -= GameplayFixedUpdateWalkHandler;
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

        private void DisableSwitchToIdleOnNextFrame()
        {
            _updatesProvider.GameplayFixedUpdate -= GameplayFixedUpdateWalkHandler;
            _idleStateRequestFlag = false;
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

        private void OnCellPositionChanged(Vector2Int cellCoords)
        {
            _idleStateRequestFlag = false;
            
            _customerView.ToWalkState(TargetModel.HasProducts);

            var deltaX = cellCoords.x - TargetModel.PreviousCellPosition.x;
            var deltaY = cellCoords.y - TargetModel.PreviousCellPosition.y;
            
            if (deltaX > 0 || deltaY < 0)
            {
                _customerView.ToRightSide();
            }
            else if (deltaX < 0 || deltaY > 0)
            {
                _customerView.ToLeftSide();
            }

            _walkContext.StartWalkPosition = _customerView.transform.position;
            _walkContext.EndWalkPosition = _gridCalculator.GetCellCenterWorld(cellCoords);
            _walkContext.Progress = 0;
            _walkContext.SteppedToNewCellFlag = false;

            _updatesProvider.GameplayFixedUpdate -= GameplayFixedUpdateWalkHandler;
            _updatesProvider.GameplayFixedUpdate += GameplayFixedUpdateWalkHandler;
        }

        private void GameplayFixedUpdateWalkHandler()
        {
            if (_idleStateRequestFlag)
            {
                _idleStateRequestFlag = false;
                
                _updatesProvider.GameplayFixedUpdate -= GameplayFixedUpdateWalkHandler;
                
                _customerView.ToIdleState(TargetModel.HasProducts);
                
                return;
            }
            
            _walkContext.Progress += Time.fixedDeltaTime * Speed;

            _customerView.transform.position = Vector3.Lerp(
                _walkContext.StartWalkPosition, _walkContext.EndWalkPosition, _walkContext.Progress);

            if (_walkContext.SteppedToNewCellFlag == false 
                && _walkContext.Progress > 0.5f
                && _gridCalculator.WorldToCell(_customerView.transform.position) == TargetModel.CellPosition)
            {
                _walkContext.SteppedToNewCellFlag = true;

                OnSteppedToNewCell();
            }

            if (_walkContext.Progress >= 1)
            {
                _idleStateRequestFlag = true;
                
                _eventBus.Dispatch(new CustomerStepFinishedEvent(TargetModel));
            }
        }

        private void OnSteppedToNewCell()
        {
            UpdateSorting(TargetModel.CellPosition);
        }

        private void UpdateSorting(Vector2Int cellCoords)
        {
            DynamicViewSortingLogic.UpdateSorting(_customerView, _ownedCellsDataHolder, cellCoords);
        }

        private class WalkContext
        {
            public Vector3 StartWalkPosition;
            public Vector3 EndWalkPosition;
            public float Progress;
            public bool SteppedToNewCellFlag;
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