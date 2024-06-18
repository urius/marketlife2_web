using Data;
using Holders;
using Infra.Instance;
using Model.ShopObjects;
using Utils;

namespace View.Game.ShopObjects.TruckPoint
{
    public class TruckPointMediator : MediatorWithModelBase<TruckPointModel>
    {
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        
        private TruckView _truckView;
        private bool _truckArrivingTriggeredFlag = false;

        protected override void MediateInternal()
        {
            _truckView = InstantiatePrefab<TruckView>(PrefabKey.ProductTruck);
            _truckView.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);
            
            SetTruckState();
            UpdateProductViews();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
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
                var hasProduct = productInBox != ProductType.Undefined;

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
            TargetModel.DeliverTimeUpdated += OnDeliverTimeUpdated;
            TargetModel.BoxRemoved += OnBoxRemoved;
        }

        private void Unsubscribe()
        {
            TargetModel.DeliverTimeUpdated -= OnDeliverTimeUpdated;
            TargetModel.BoxRemoved -= OnBoxRemoved;
        }

        private void OnBoxRemoved(int boxIndex)
        {
            
        }

        private void SetTruckState()
        {
            switch (TargetModel.DeliverTimeSecondsRest)
            {
                case <= 0:
                    SetTruckArrived();
                    break;
                case <= Constants.TruckArrivingDuration:
                    AnimateTruckArrive();
                    break;
                default:
                    SetTruckMovedOut();
                    break;
            }
        }

        private void OnDeliverTimeUpdated(int deliverTimeRest)
        {
            if (deliverTimeRest <= Constants.TruckArrivingDuration
                && _truckArrivingTriggeredFlag == false)
            {
                AnimateTruckArrive();
            }
        }

        private void AnimateTruckArrive()
        {
            _truckArrivingTriggeredFlag = true;
            
            _truckView.AnimateTruckArrive();
        }

        private void SetTruckArrived()
        {
            _truckArrivingTriggeredFlag = true;
            
            _truckView.SetTruckArrived();
        }

        private void SetTruckMovedOut()
        {
            _truckArrivingTriggeredFlag = false;
            
            _truckView.SetTruckMovedOut();
        }
    }
}