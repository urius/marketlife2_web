using System.Collections.Generic;
using System.Linq;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using UnityEngine;

namespace Systems
{
    public class TruckPointsLogicSystem : ISystem
    {
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
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
                var isAdvanced = truckPointModel.AdvanceDeliverTime();
                
                if (isAdvanced && truckPointModel.DeliverTimeSecondsRest <= 0)
                {
                    _eventBus.Dispatch(new TruckArrivedEvent(truckPointModel));
                    return;
                }

                if (truckPointModel.DeliverTimeSecondsRest <= 0 
                    && truckPointModel.HasProducts == false)
                {
                    truckPointModel.ResetDeliverTime();
                    truckPointModel.ResetProductsSilently();
                }
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