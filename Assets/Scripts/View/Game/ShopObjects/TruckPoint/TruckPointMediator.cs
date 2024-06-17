using Data;
using Infra.Instance;
using Model.ShopObjects;
using Utils;

namespace View.Game.ShopObjects.TruckPoint
{
    public class TruckPointMediator : MediatorWithModelBase<TruckPointModel>
    {
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        
        private TruckView _truckView;
        private bool _truckArrivingTriggeredFlag = false;

        protected override void MediateInternal()
        {
            _truckView = InstantiatePrefab<TruckView>(PrefabKey.ProductTruck);
            _truckView.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);
            
            SetTruckState();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_truckView);
        }

        private void Subscribe()
        {
            TargetModel.DelivarTimeUpdated += OnDeliverTimeUpdated;
        }

        private void Unsubscribe()
        {
            TargetModel.DelivarTimeUpdated -= OnDeliverTimeUpdated;
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