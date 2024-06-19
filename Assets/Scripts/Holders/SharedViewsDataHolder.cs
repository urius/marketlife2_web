using System.Collections.Generic;
using Model.ShopObjects;
using View.Game.ShopObjects.TruckPoint;

namespace Holders
{
    public class SharedViewsDataHolder : ISharedViewsDataHolder
    {
        private readonly Dictionary<TruckPointModel, ITruckBoxPositionsProvider> _dataDictionary = new();

        public void RegisterTruckBoxPositionProvider(TruckPointModel model, ITruckBoxPositionsProvider provider)
        {
            _dataDictionary[model] = provider;
        }

        public void UnregisterTruckBoxPositionProvider(TruckPointModel model)
        {
            _dataDictionary.Remove(model);
        }

        public ITruckBoxPositionsProvider GetPositionsProvider(TruckPointModel model)
        {
            return _dataDictionary.TryGetValue(model, out var provider) ? provider : null;
        }
    }

    public interface ISharedViewsDataHolder
    {
        public void RegisterTruckBoxPositionProvider(TruckPointModel model, ITruckBoxPositionsProvider provider);
        public ITruckBoxPositionsProvider GetPositionsProvider(TruckPointModel model);
        void UnregisterTruckBoxPositionProvider(TruckPointModel model);
    }
}