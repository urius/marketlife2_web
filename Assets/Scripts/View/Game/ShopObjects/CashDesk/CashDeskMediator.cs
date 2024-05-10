using Data;
using Infra.Instance;
using Model.ShopObjects;
using Utils;

namespace View.Game.ShopObjects.CashDesk
{
    public class CashDeskMediator : MediatorWithModelBase<CashDeskModel>
    {
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        
        private CashDeskView _view;

        protected override void MediateInternal()
        {
            var go = InstantiatePrefab(PrefabKey.CashDesk);
            _view = go.GetComponent<CashDeskView>();

            _view.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);
        }

        protected override void UnmediateInternal()
        {
            Destroy(_view);
            _view = null;
        }
    }
}