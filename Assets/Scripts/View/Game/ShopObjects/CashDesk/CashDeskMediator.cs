using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using Model.ShopObjects;
using Utils;
using View.Game.Extensions;
using View.Game.Misc;
using View.Helpers;

namespace View.Game.ShopObjects.CashDesk
{
    public class CashDeskMediator : ShopObjectMediatorBase<CashDeskModel>
    {
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        
        private readonly LinkedList<MoneyView> _moneyItems = new();

        private CashDeskView _view;

        protected override void MediateInternal()
        {
            var go = InstantiatePrefab(PrefabKey.CashDesk);
            _view = go.GetComponent<CashDeskView>();
            _view.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);

            _sharedViewsDataHolder.RegisterCashDeskMoneyPositionProvider(TargetModel, _view);
            OwnCells();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();

            _sharedViewsDataHolder.UnregisterCashDeskMoneyPositionProvider(TargetModel);
            _ownedCellsDataHolder.UnregisterShopObject(TargetModel);
            
            Destroy(_view);
            _view = null;
        }

        protected override void UpdateSorting()
        {
            _view.SetSortingOrder(SortingOrderHelper.GetDefaultSortingOrderByCoords(TargetModel.CellCoords));
        }

        private void OwnCells()
        {
            var ownedCells = _gridCalculator.GetOwnedCells(_view);

            _ownedCellsDataHolder.RegisterShopObject(TargetModel, ownedCells);
        }

        private void Subscribe()
        {
            TargetModel.MoneyAdded += OnMoneyAdded;
            TargetModel.MoneyReset += OnMoneyReset;
        }

        private void Unsubscribe()
        {
            TargetModel.MoneyAdded -= OnMoneyAdded;
            TargetModel.MoneyReset -= OnMoneyReset;
        }

        private void OnMoneyAdded()
        {
            ShowMoney(TargetModel.MoneyAmount);
        }

        private void OnMoneyReset()
        {
            ClearMoneyItems();
        }

        private void ClearMoneyItems()
        {
            foreach (var moneyView in _moneyItems)
            {
                ReturnToCache(moneyView.gameObject);
            }

            _moneyItems.Clear();
        }

        private void ShowMoney(int moneyAmount)
        {
            var moneyItemsAmount = _moneyItems.Count;
            
            if (moneyItemsAmount >= moneyAmount) return;

            for (var i = moneyItemsAmount; i < moneyAmount; i++)
            {
                var moneyView = GetFromCache<MoneyView>(PrefabKey.MoneyCashDesk);
                _moneyItems.AddLast(moneyView);
                _view.PlaceToMoneyPosition(moneyView, i);
            }
        }
    }
}