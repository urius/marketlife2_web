using System.Linq;
using Data;
using Holders;
using Infra.Instance;
using Model.ShopObjects;
using Utils;
using View.Helpers;

namespace View.Game.ShopObjects.CashDesk
{
    public class CashDeskMediator : ShopObjectMediatorBase<CashDeskModel>
    {
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        
        private CashDeskView _view;

        protected override void MediateInternal()
        {
            var go = InstantiatePrefab(PrefabKey.CashDesk);
            _view = go.GetComponent<CashDeskView>();

            _view.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);

            OwnCells();
        }

        private void OwnCells()
        {
            var ownedCells = _view.OwnedCellViews
                .Select(v => _gridCalculator.WorldToCell(v.transform.position))
                .ToArray();

            _ownedCellsDataHolder.RegisterShopObject(TargetModel, ownedCells);
        }

        protected override void UnmediateInternal()
        {
            Destroy(_view);
            _view = null;
        }

        protected override void UpdateSorting()
        {
            _view.SetSortingOrder(SortingOrderHelper.GetDefaultSortingOrderByCoords(TargetModel.CellCoords));
        }
    }
}