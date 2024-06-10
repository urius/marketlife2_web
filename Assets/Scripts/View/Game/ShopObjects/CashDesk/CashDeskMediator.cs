using Data;
using Holders;
using Infra.Instance;
using Model.ShopObjects;
using Utils;
using View.Game.Extensions;
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
            var ownedCells = _gridCalculator.GetOwnedCells(_view);

            _ownedCellsDataHolder.RegisterShopObject(TargetModel, ownedCells);
        }

        protected override void UnmediateInternal()
        {
            _ownedCellsDataHolder.UnregisterShopObject(TargetModel);
            
            Destroy(_view);
            _view = null;
        }

        protected override void UpdateSorting()
        {
            _view.SetSortingOrder(SortingOrderHelper.GetDefaultSortingOrderByCoords(TargetModel.CellCoords));
        }
    }
}