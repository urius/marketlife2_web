using System.Collections.Generic;
using Model.ShopObjects;
using View.Game.People;
using View.Game.ShopObjects.CashDesk;
using View.Game.ShopObjects.Shelf;
using View.Game.ShopObjects.TruckPoint;

namespace Holders
{
    public class SharedViewsDataHolder : ISharedViewsDataHolder
    {
        private readonly Dictionary<TruckPointModel, ITruckBoxPositionsProvider> _truckPointsDictionary = new();
        private readonly Dictionary<ShelfModel, IShelfProductSlotPositionProvider> _shelfSlotsDictionary = new();
        private readonly Dictionary<CashDeskModel, ICashDeskMoneyPositionProvider> _cashDeskProvidersDictionary = new();

        private IPlayerCharPositionsProvider _playerCharPositionsProvider;

        public void RegisterTruckBoxPositionProvider(TruckPointModel model, ITruckBoxPositionsProvider provider)
        {
            _truckPointsDictionary[model] = provider;
        }

        public void UnregisterTruckBoxPositionProvider(TruckPointModel model)
        {
            _truckPointsDictionary.Remove(model);
        }

        public ITruckBoxPositionsProvider GetTruckBoxPositionsProvider(TruckPointModel model)
        {
            return _truckPointsDictionary.TryGetValue(model, out var provider) ? provider : null;
        }

        public void RegisterPlayerCharPositionProvider(IPlayerCharPositionsProvider provider)
        {
            _playerCharPositionsProvider = provider;
        }

        public IPlayerCharPositionsProvider GetPlayerCharPositionProvider()
        {
            return _playerCharPositionsProvider;
        }

        public void UnregisterPlayerCharPositionProvider()
        {
            _playerCharPositionsProvider = null;
        }

        public void RegisterShelfSlotPositionProvider(ShelfModel model, IShelfProductSlotPositionProvider provider)
        {
            _shelfSlotsDictionary[model] = provider;
        }

        public IShelfProductSlotPositionProvider GetShelfSlotPositionProvider(ShelfModel model)
        {
            return _shelfSlotsDictionary.TryGetValue(model, out var provider) ? provider : null;
        }

        public void UnregisterShelfSlotPositionProvider(ShelfModel model)
        {
            _shelfSlotsDictionary.Remove(model);
        }


        public void RegisterCashDeskMoneyPositionProvider(CashDeskModel model, ICashDeskMoneyPositionProvider provider)
        {
            _cashDeskProvidersDictionary[model] = provider;
        }

        public ICashDeskMoneyPositionProvider GetCashDeskMoneyPositionProvider(CashDeskModel model)
        {
            return _cashDeskProvidersDictionary.TryGetValue(model, out var provider) ? provider : null;
        }

        public void UnregisterCashDeskMoneyPositionProvider(CashDeskModel model)
        {
            _cashDeskProvidersDictionary.Remove(model);
        }
    }

    public interface ISharedViewsDataHolder
    {
        public void RegisterTruckBoxPositionProvider(TruckPointModel model, ITruckBoxPositionsProvider provider);
        public ITruckBoxPositionsProvider GetTruckBoxPositionsProvider(TruckPointModel model);
        public void UnregisterTruckBoxPositionProvider(TruckPointModel model);
        
        public void RegisterPlayerCharPositionProvider(IPlayerCharPositionsProvider provider);
        public IPlayerCharPositionsProvider GetPlayerCharPositionProvider();
        public void UnregisterPlayerCharPositionProvider();
        
        public void RegisterShelfSlotPositionProvider(ShelfModel model, IShelfProductSlotPositionProvider provider);
        public IShelfProductSlotPositionProvider GetShelfSlotPositionProvider(ShelfModel model);
        public void UnregisterShelfSlotPositionProvider(ShelfModel model);
        
        public void RegisterCashDeskMoneyPositionProvider(CashDeskModel model, ICashDeskMoneyPositionProvider provider);
        public ICashDeskMoneyPositionProvider GetCashDeskMoneyPositionProvider(CashDeskModel model);
        public void UnregisterCashDeskMoneyPositionProvider(CashDeskModel model);
    }
}