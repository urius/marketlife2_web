using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.People;
using Model.People.States;
using Model.People.States.Staff;
using Model.ShopObjects;
using UnityEngine;
using View.Game.Misc;
using View.Game.Shared;
using View.Helpers;

namespace View.Game.People
{
    public class TruckPointStaffCharMediator : BotCharMediatorBase<TruckPointStaffCharModel>
    {
        private static readonly Color[] TimeProgressColors = { Color.red, Color.yellow, Color.yellow, Color.yellow, Color.green,Color.green, Color.green };

        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        
        private TakeBoxAnimationContext _takeBoxAnimationContext;
        private ManClockView _clockView;
        private PlayerModel _playerModel;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            
            base.MediateInternal();

            _clockView = InstantiatePrefab<ManClockView>(PrefabKey.ManClockIcon);
            
            SetClothes();
            UpdateClockIconColor();
            UpdateClockPosition();
            
            Subscribe();
            
            _sharedViewsDataHolder.RegisterCharProductsInBoxPositionsProvider(TargetModel.ProductsBox, ManView);
        }

        protected override void UnmediateInternal()
        {
            _sharedViewsDataHolder.UnregisterCharProductsInBoxPositionsProvider(TargetModel.ProductsBox);
            
            Unsubscribe();
            
            Destroy(_clockView);
            _clockView = null;
            
            base.UnmediateInternal();
        }

        protected override void ToWalkingState()
        {
            ManView.ToWalkState(TargetModel.HasProducts);
        }

        protected override void ToIdleState()
        {
            ManView.ToIdleState(TargetModel.HasProducts);
        }

        protected override void StepFinishedHandler()
        {
            _eventBus.Dispatch(new TrucPointStaffStepFinishedEvent(TargetModel));
        }

        protected override void ProcessWalk()
        {
            base.ProcessWalk();

            UpdateClockPosition();
        }

        private void UpdateClockPosition()
        {
            _clockView.transform.position = ManView.transform.position;
        }

        private void SetClothes()
        {
            var clothes = ManSpriteTypesHelper.GetTruckPointStaffClothes();
            var hair = ManSpriteTypesHelper.GetRandomHair();
            
            SetBaseClothes(clothes.BodyClothes, clothes.HandClothes, clothes.FootClothes, hair);
            SetHat(ManSpriteType.None);

            SetRandomGlasses();
        }

        private void Subscribe()
        {
            TargetModel.ProductsBoxAdded += OnProductsBoxAdded;
            TargetModel.ProductRemoved += OnProductRemoved;
            TargetModel.StateChanged += OnStateChanged;
            TargetModel.WorkSecondsLeftChanged += OnWorkSecondsLeftChanged;
        }

        private void Unsubscribe()
        {
            TargetModel.ProductsBoxAdded -= OnProductsBoxAdded;
            TargetModel.ProductRemoved -= OnProductRemoved;
            TargetModel.StateChanged -= OnStateChanged;
            TargetModel.WorkSecondsLeftChanged -= OnWorkSecondsLeftChanged;
        }

        private void OnWorkSecondsLeftChanged(int workSecondsLeft)
        {
            UpdateClockIconColor();
        }

        private void UpdateClockIconColor()
        {
            
            var color = StaffCharHelper.GetClockColorByPercent((float)TargetModel.WorkSecondsLeft / _playerModel.StaffWorkTimeSeconds);
            _clockView.SetIconColor(color);
        }

        private void OnProductsBoxAdded()
        {
            UpdateProductBoxView();
        }

        private void OnProductRemoved(int slotIndex)
        {
            ManView.SetProductsBoxVisibility(TargetModel.HasProducts);
            DisplayProduct(slotIndex);
        }

        private void UpdateProductBoxView()
        {
            var hasProducts = TargetModel.HasProducts;
            ManView.SetProductsBoxVisibility(hasProducts);

            if (hasProducts)
            {
                for (var i = 0; i < TargetModel.ProductsInBox.Count; i++)
                {
                    DisplayProduct(i);
                }
            }
        }

        private void DisplayProduct(int slotIndex)
        {
            var sprite = _spritesHolderSo.GetProductSpriteByKey(TargetModel.ProductsInBox[slotIndex]);
            ManView.SetProductInBoxSprite(slotIndex, sprite);
        }

        private void OnStateChanged(BotCharStateBase state)
        {
            if (state.StateName == ShopCharStateName.TpStaffTakingProductsFromTruckPoint)
            {
                var takeProductState = (StaffTakeProductFromTruckPointState)state;
                AnimateTakeBoxFromTruck(takeProductState.TruckPointModel,
                    takeProductState.ProductBoxIndexToTake);
            }
        }

        private void AnimateTakeBoxFromTruck(TruckPointModel truckPointModel, int productBoxIndexToTake)
        {
            var startPosition = _sharedViewsDataHolder
                .GetTruckPointBoxPositions(truckPointModel, productBoxIndexToTake);

            var animatedBoxView = InstantiatePrefab<ProductsBoxView>(PrefabKey.ProductsBox);
            var productType = TargetModel.ProductsInBox[0];
            
            _takeBoxAnimationContext = AnimationHelper.InitTakeBoxFromTruckPointAnimation(ManView, animatedBoxView, productType, startPosition);

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
                
                _eventBus.Dispatch(new StaffTakeBoxFromTruckAnimationFinishedEvent(TargetModel));
            }
        }
    }
}