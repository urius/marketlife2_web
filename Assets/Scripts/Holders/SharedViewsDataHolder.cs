using System.Collections.Generic;
using Model.ShopObjects;
using View.Game.People;
using View.Game.ShopObjects.TruckPoint;

namespace Holders
{
    public class SharedViewsDataHolder : ISharedViewsDataHolder
    {
        private readonly Dictionary<TruckPointModel, ITruckBoxPositionsProvider> _dataDictionary = new();
        private IManViewBoxProductsPositionsProvider _playerCharBoxProductsPositionsProvider;

        public void RegisterTruckBoxPositionProvider(TruckPointModel model, ITruckBoxPositionsProvider provider)
        {
            _dataDictionary[model] = provider;
        }

        public void UnregisterTruckBoxPositionProvider(TruckPointModel model)
        {
            _dataDictionary.Remove(model);
        }

        public ITruckBoxPositionsProvider GetTruckBoxPositionsProvider(TruckPointModel model)
        {
            return _dataDictionary.TryGetValue(model, out var provider) ? provider : null;
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
    }

    public interface ISharedViewsDataHolder
    {
        public void RegisterTruckBoxPositionProvider(TruckPointModel model, ITruckBoxPositionsProvider provider);
        public ITruckBoxPositionsProvider GetTruckBoxPositionsProvider(TruckPointModel model);
        public void UnregisterTruckBoxPositionProvider(TruckPointModel model);
        
        public void RegisterPlayerCharBoxProductsPositionProvider(IManViewBoxProductsPositionsProvider provider);
        public IManViewBoxProductsPositionsProvider GetPlayerCharBoxPositionProvider();
        public void UnregisterPlayerCharBoxPositionProvider();
    }
}