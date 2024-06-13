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

        protected override void MediateInternal()
        {
            _truckView = InstantiatePrefab<TruckView>(PrefabKey.ProductTruck);

            _truckView.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);

            _truckView.AnimateTruckArrive();
        }

        protected override void UnmediateInternal()
        {
            Destroy(_truckView);
        }
    }
}