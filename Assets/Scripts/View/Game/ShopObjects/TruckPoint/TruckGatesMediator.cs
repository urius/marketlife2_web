using Data;
using Infra.Instance;
using Model.ShopObjects;
using Utils;

namespace View.Game.ShopObjects.TruckPoint
{
    public class TruckGatesMediator : MediatorWithModelBase<TruckPointModel>
    {
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        
        private TruckGatesView _truckGatesView;
        private bool _isOpened = false;

        protected override void MediateInternal()
        {
            _truckGatesView = InstantiatePrefab<TruckGatesView>(PrefabKey.TruckGates);

            var pos = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);
            
            _truckGatesView.transform.position = pos;

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_truckGatesView);
        }

        private void Subscribe()
        {
            TargetModel.DeliverTimeAdvanced += OnDeliverTimeAdvanced;
        }
        
        private void Unsubscribe()
        {
            TargetModel.DeliverTimeAdvanced -= OnDeliverTimeAdvanced;
        }

        private void OnDeliverTimeAdvanced(int timeLeft)
        {
            if (timeLeft <= Constants.TruckArrivingDuration)
            {
                SetOpenedIfNeeded();
            }
            else
            {
                SetClosedIfNeeded();
            }
        }

        private void SetOpenedIfNeeded()
        {
            if (_isOpened) return;

            _isOpened = true;
            _truckGatesView.AnimateOpenDoors();
        }

        private void SetClosedIfNeeded()
        {
            if (!_isOpened) return;

            _isOpened = false;
            _truckGatesView.AnimateCloseDoors();
        }
    }
}