using System.Collections.Generic;
using System.IO;
using Data;
using Events;
using Extensions;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model.People;
using Model.ShopObjects;
using Tools.AudioManager;
using UnityEngine;
using Utils;
using View.Game.Extensions;
using View.Game.Product;
using View.Helpers;

namespace View.Game.ShopObjects.Shelf
{
    public class ShelfMediator : ShopObjectMediatorBase<ShelfModel>
    {
        private const float PutProductDuration = 0.4f;
        private const float PutProductHalfDuration = PutProductDuration * 0.5f;
        
        private readonly IShelfUpgradeSettingsProvider _shelfUpgradeSettingsProvider = Instance.Get<IShelfUpgradeSettingsProvider>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly Queue<PutProductAnimationContext> _putProductAnimationContexts = new();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();

        private ShelfView _view;

        protected override void MediateInternal()
        {
            if (_shelfUpgradeSettingsProvider.TryGetShelfUpgradeSetting(TargetModel.ShopObjectType, TargetModel.UpgradeIndex, out var shelfSettings))
            {
                CreateView(shelfSettings);

                DisplayProducts();

                Subscribe();
            }
            else
            {
                throw new InvalidDataException(
                    $"Unable to get shelf setting for shelf type: {TargetModel.ShopObjectType}");
            }
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            DestroyView();
        }

        private void CreateView(ShelfUpgradeSettingsProvider.ShelfUpgradeSettingsProviderData shelfSettings)
        {
            _view = InstantiatePrefab<ShelfView>(shelfSettings.PrefabKey);
            _sharedViewsDataHolder.RegisterShelfSlotPositionProvider(TargetModel, _view);
            
            _view.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);
            
            UpdateSorting();
            
            OwnCells();
        }

        private void DestroyView()
        {
            _sharedViewsDataHolder.UnregisterShelfSlotPositionProvider(TargetModel);
            _ownedCellsDataHolder.UnregisterShopObject(TargetModel);
            
            Destroy(_view);
            _view = null;
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<AnimatePutProductOnShelfEvent>(AnimatePutProductOnShelfEventHandler);
            TargetModel.ProductAdded += OnProductAdded;
            TargetModel.ProductRemoved += OnProductRemoved;
            TargetModel.UpgradeIndexChanged += OnUpgradeIndexChanged;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<AnimatePutProductOnShelfEvent>(AnimatePutProductOnShelfEventHandler);
            TargetModel.ProductAdded -= OnProductAdded;
            TargetModel.ProductRemoved -= OnProductRemoved;
            TargetModel.UpgradeIndexChanged -= OnUpgradeIndexChanged;
        }

        private void OnUpgradeIndexChanged(int index)
        {
            if (_shelfUpgradeSettingsProvider.TryGetShelfUpgradeSetting(TargetModel.ShopObjectType,
                    TargetModel.UpgradeIndex, out var shelfSettings))
            {
                DestroyView();

                CreateView(shelfSettings);

                DisplayProducts();

                _eventBus.Dispatch(new VFXRequestSmokeEvent(TargetModel.CellCoords + Vector2Int.left));
            }
        }

        private void AnimatePutProductOnShelfEventHandler(AnimatePutProductOnShelfEvent e)
        {
            if (e.ShelfModel != TargetModel) return;

            if (e.IsPlayerCharAction)
            {
                _audioPlayer.PlaySound(SoundIdKey.PutProductOnShelf);
            }

            var shelfSlotIndex = e.ShelfSlotIndex;
            var productView = GetFromCache<ProductView>(PrefabKey.ProductView);

            var productSprite = _spritesHolderSo.GetProductSpriteByKey(e.ProductToMove);
            productView.SetSortingLayerName(Constants.GeneralTopSortingLayerName);
            productView.SetSprite(productSprite);

            var putProductAnimationContext = new PutProductAnimationContext()
            {
                ProductView = productView,
                ShelfSlotIndex = shelfSlotIndex,
                TargetProductsBoxModel = e.ProductBoxModel,
            };
            _putProductAnimationContexts.Enqueue(putProductAnimationContext);

            var productsInBoxPositionsProvider = _sharedViewsDataHolder.GetCharProductsInBoxPositionsProvider(e.ProductBoxModel);
            productView.transform.position = productsInBoxPositionsProvider.GetProductInBoxPosition(e.BoxSlotIndex);

            var targetPosition = _view.GetSlotPosition(shelfSlotIndex);
            _view.SetProductSprite(shelfSlotIndex, null);

            LeanTween
                .delayedCall(_view.gameObject, PutProductHalfDuration, OnPutProductHalfAnimation)
                .setOnCompleteParam(putProductAnimationContext);
            
            productView.transform.LeanMove(targetPosition, PutProductDuration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(OnAnimatePutProductComplete);
        }

        private void OnPutProductHalfAnimation(object putProductAnimationContext)
        {
            var context = (PutProductAnimationContext)putProductAnimationContext;
            _eventBus.Dispatch(new PutProductOnShelfHalfAnimationEvent(context.TargetProductsBoxModel));
        }

        private void OnAnimatePutProductComplete()
        {
            var putProductAnimationContext = _putProductAnimationContexts.Dequeue();
            
            DisplayProduct(putProductAnimationContext.ShelfSlotIndex);
            ReturnToCache(putProductAnimationContext.ProductView.gameObject);
        }

        private void OwnCells()
        {
            var ownedCells = _gridCalculator.GetOwnedCells(_view);

            _ownedCellsDataHolder.RegisterShopObject(TargetModel, ownedCells);
        }

        private void DisplayProducts()
        {
            for (var i = 0; i < TargetModel.ProductSlots.Length; i++)
            {
                DisplayProduct(i);
            }
        }

        private void DisplayProduct(int slotIndex)
        {
            var productType = TargetModel.ProductSlots[slotIndex];
            var productSprite = _spritesHolderSo.GetProductSpriteByKey(productType);

            _view.SetProductSprite(slotIndex, productSprite);
        }

        private void OnProductAdded(int slotIndex)
        {
            DisplayProduct(slotIndex);
        }

        private void OnProductRemoved(int slotIndex)
        {
            DisplayProduct(slotIndex);
        }

        protected override void UpdateSorting()
        {
            _view.SetSortingOrder(SortingOrderHelper.GetDefaultSortingOrderByCoords(TargetModel.CellCoords));
        }


        private class PutProductAnimationContext
        {
            public ProductView ProductView;
            public ProductBoxModel TargetProductsBoxModel;
            public int ShelfSlotIndex;
        }
    }
}