using System.Collections.Generic;
using Model.ShopObjects;
using View.Game.People;
using View.Game.ShopObjects.Shelf;
using View.Game.ShopObjects.TruckPoint;

namespace Holders
{
    public class SharedViewsDataHolder : ISharedViewsDataHolder
    {
        private readonly Dictionary<TruckPointModel, ITruckBoxPositionsProvider> _truckPointsDictionary = new();
        private readonly Dictionary<ShelfModel, IShelfProductSlotPositionProvider> _shelfSlotsDictionary = new();
        
        private IManViewBoxProductsPositionsProvider _playerCharBoxProductsPositionsProvider;

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

        public void RegisterPlayerCharBoxProductsPositionProvider(IManViewBoxProductsPositionsProvider provider)
        {
            _playerCharBoxProductsPositionsProvider = provider;
        }

        public IManViewBoxProductsPositionsProvider GetPlayerCharBoxPositionProvider()
        {
            return _playerCharBoxProductsPositionsProvider;
        }

        public void UnregisterPlayerCharBoxPositionProvider()
        {
            _playerCharBoxProductsPositionsProvider = null;
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
    }

    public interface ISharedViewsDataHolder
    {
        public void RegisterTruckBoxPositionProvider(TruckPointModel model, ITruckBoxPositionsProvider provider);
        public ITruckBoxPositionsProvider GetTruckBoxPositionsProvider(TruckPointModel model);
        public void UnregisterTruckBoxPositionProvider(TruckPointModel model);
        
        public void RegisterPlayerCharBoxProductsPositionProvider(IManViewBoxProductsPositionsProvider provider);
        public IManViewBoxProductsPositionsProvider GetPlayerCharBoxPositionProvider();
        public void UnregisterPlayerCharBoxPositionProvider();
        
        public void RegisterShelfSlotPositionProvider(ShelfModel model, IShelfProductSlotPositionProvider provider);
        public IShelfProductSlotPositionProvider GetShelfSlotPositionProvider(ShelfModel model);
        public void UnregisterShelfSlotPositionProvider(ShelfModel model);
    }
}