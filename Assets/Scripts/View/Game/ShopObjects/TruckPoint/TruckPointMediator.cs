using Cysharp.Threading.Tasks;
using Data;
using Events;
using Extensions;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using Tools.AudioManager;
using UnityEngine;
using Utils;

namespace View.Game.ShopObjects.TruckPoint
{
    public class TruckPointMediator : MediatorWithModelBase<TruckPointModel>
    {
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        
        private TruckView _truckView;
        private bool _truckArrivingTriggeredFlag = false;
        private PlayerCharModel _playerCharModel;

        protected override void MediateInternal()
        {
            _playerCharModel = _playerModelHolder.PlayerCharModel;
            
            _truckView = InstantiatePrefab<TruckView>(PrefabKey.ProductTruck);
            _truckView.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);
            
            _sharedViewsDataHolder.RegisterTruckBoxPositionProvider(TargetModel, _truckView);
            
            SetTruckStateInitial();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            _sharedViewsDataHolder.UnregisterTruckBoxPositionProvider(TargetModel);
            
            Unsubscribe();
            
            Destroy(_truckView);
        }

        private void UpdateProductViews()
        {
            for (var i = 0; i < _truckView.ProductBoxesAmount; i++)
            {
                UpdateProductView(i);
            }
        }

        private void UpdateProductView(int index)
        {
            var productInBox = TargetModel.GetProductTypeAtBoxIndex(index);

            if (_truckView.TryGetProductBoxView(index, out var boxView))
            {
                var hasProduct = productInBox != ProductType.None;

                boxView.SetVisible(hasProduct);

                if (hasProduct)
                {
                    var productSprite = _spritesHolderSo.GetProductSpriteByKey(productInBox);
                    boxView.SetProductsSprite(productSprite);
                }
            }
        }

        private void Subscribe()
        {
            TargetModel.DeliverTimeAdvanced += OnDeliverTimeAdvanced;
            TargetModel.DeliverTimeReset += OnDeliverTimeReset;
            TargetModel.ProductsReset += OnProductsReset;
            TargetModel.BoxRemoved += OnBoxRemoved;
        }

        private void Unsubscribe()
        {
            TargetModel.DeliverTimeAdvanced -= OnDeliverTimeAdvanced;
            TargetModel.DeliverTimeReset -= OnDeliverTimeReset;
            TargetModel.ProductsReset -= OnProductsReset;
            TargetModel.BoxRemoved -= OnBoxRemoved;
        }

        private void OnBoxRemoved(int boxIndex)
        {
            UpdateProductView(boxIndex);
        }

        private void SetTruckStateInitial()
        {
            switch (TargetModel.DeliverTimeSecondsRest)
            {
                case <= 0:
                    SetTruckArrived();
                    break;
                case <= Constants.TruckArrivingDuration + 1:
                    AnimateTruckArrive().Forget();
                    break;
                default:
                    SetTruckMovedOut();
                    break;
            }
        }

        private void OnDeliverTimeAdvanced(int deliverTimeRest)
        {
            if (deliverTimeRest <= Constants.TruckArrivingDuration
                && _truckArrivingTriggeredFlag == false)
            {
                AnimateTruckArrive().Forget();
            }
        }

        private void OnDeliverTimeReset()
        {
            _truckArrivingTriggeredFlag = false;
            AnimateTruckMovedOut();
        }

        private void OnProductsReset()
        {
            UpdateProductViews();
        }

        private async UniTaskVoid AnimateTruckArrive()
        {
            _truckArrivingTriggeredFlag = true;

            if (CheckPlayerCloseEnough())
            {
                _audioPlayer.PlaySound(SoundIdKey.TruckIn);
            }
            
            await _truckView.AnimateTruckArrive();
            
            _eventBus.Dispatch(new TruckArriveAnimationFinishedEvent(TargetModel));
            
            _truckView.AnimateCapOpen();
        }

        private void SetTruckArrived()
        {
            _truckArrivingTriggeredFlag = true;
            
            UpdateProductViews();
            
            _truckView.SetTruckArrived();
        }

        private void AnimateTruckMovedOut()
        {
            _truckArrivingTriggeredFlag = false;

            PlayTruckOutSoundWithDelay().Forget();
            
            _truckView.AnimateTruckMovedOut();
        }

        private async UniTaskVoid PlayTruckOutSoundWithDelay()
        {
            if (CheckPlayerCloseEnough())
            {
                await UniTask.Delay((int)(TruckView.CapAnimationDuration * 1000));
            
                _audioPlayer.PlaySound(SoundIdKey.TruckOut);
            }
        }

        private bool CheckPlayerCloseEnough()
        {
            return Vector2Int.Distance(_playerCharModel.CellPosition, TargetModel.CellCoords) <= Constants.CellDistanceToSound;
        }

        private void SetTruckMovedOut()
        {
            _truckArrivingTriggeredFlag = false;
            
            _truckView.SetTruckMovedOut();
        }
    }
}