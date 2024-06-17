using System.Collections.Generic;
using System.Linq;
using Data;
using Holders;
using Infra.Instance;
using Model;
using Model.ShopObjects;

namespace Systems
{
    public class TruckPointsLogicSystem : ISystem
    {
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly List<TruckPointModel> _truckPointList = new();

        private ShopModel _shopModel;

        public void Start()
        {
            _shopModel = _shopModelHolder.ShopModel;
                
            PopulateTruckPointModels();

            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _shopModel.ShopObjectAdded += OnShopObjectAdded;
            _updatesProvider.SecondPassed += OnSecondPassed;
        }

        private void Unsubscribe()
        {
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
            _updatesProvider.SecondPassed -= OnSecondPassed;
        }

        private void OnSecondPassed()
        {
            foreach (var truckPointModel in _truckPointList)
            {
                truckPointModel.AdvanceDeliverTime();
            }
        }

        private void OnShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            if (shopObjectModel.ShopObjectType == ShopObjectType.TruckPoint)
            {
                _truckPointList.Add((TruckPointModel)shopObjectModel);
            }
        }

        private void PopulateTruckPointModels()
        {
            var truckPoints = _shopModel.ShopObjects.Values
                .Where(o => o.ShopObjectType == ShopObjectType.TruckPoint)
                .Cast<TruckPointModel>()
                .ToArray();

            _truckPointList.Capacity = truckPoints.Length * 2;
            
            _truckPointList.AddRange(truckPoints);
        }
    }
}