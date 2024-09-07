using System.Collections.Generic;
using System.Linq;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.People;
using Model.ShopObjects;
using UnityEngine;

namespace View.Game.People
{
    public class BotCharsMediator : MediatorBase
    {
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private readonly Dictionary<CustomerCharModel, CustomerCharMediator> _childCostumerMediators = new();
        private readonly Dictionary<TruckPointStaffCharModel, TruckPointStaffCharMediator> _childTruckPointStaffMediators = new();

        private CustomersModel _customersModel;
        private ShopModel _shopModel;
        private List<TruckPointModel> _truckPointsWatchList;

        protected override void MediateInternal()
        {
            _shopModel = _shopModelHolder.ShopModel;
            _customersModel = _shopModelHolder.ShopModel.CustomersModel;
            
            _truckPointsWatchList = _shopModel.GetTruckPointModels().ToList();
            _truckPointsWatchList.ForEach(HandleTruckPointStaffMediators);
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _customersModel.CustomerAdded += OnCustomerAdded;
            _customersModel.CustomerRemoved += OnCustomerRemoved;
            _shopModel.ShopObjectAdded += OnShopObjectAdded;
            
            foreach (var truckPointModel in _truckPointsWatchList)
            {
                SubscribeOnTruckPoint(truckPointModel);
            }
        }

        private void Unsubscribe()
        {
            foreach (var truckPointModel in _truckPointsWatchList)
            {
                UnsubscribeFromTruckPoint(truckPointModel);
            }
            
            _customersModel.CustomerAdded -= OnCustomerAdded;
            _customersModel.CustomerRemoved -= OnCustomerRemoved;
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
        }

        private void SubscribeOnTruckPoint(TruckPointModel truckPointModel)
        {
            truckPointModel.StaffAdded += OnStaffAdded;
            truckPointModel.StaffRemoved += OnStaffRemoved;
        }

        private void UnsubscribeFromTruckPoint(TruckPointModel truckPointModel)
        {
            truckPointModel.StaffAdded -= OnStaffAdded;
            truckPointModel.StaffRemoved -= OnStaffRemoved;
        }

        private void OnStaffAdded(TruckPointStaffCharModel charModel)
        {
            _eventBus.Dispatch(new VFXRequestSmokeEvent(charModel.CellCoords));
            
            MediateTruckPointStaff(charModel);
        }

        private void OnStaffRemoved(TruckPointStaffCharModel charModel)
        {
            UnmediateTruckPointStaff(charModel);
            
            _eventBus.Dispatch(new VFXRequestSmokeEvent(charModel.CellCoords));
        }

        private void HandleTruckPointStaffMediators(TruckPointModel truckPointModel)
        {
            if (truckPointModel.HasStaff)
            {
                MediateTruckPointStaff(truckPointModel.StaffCharModel);
            }
        }

        private void MediateTruckPointStaff(TruckPointStaffCharModel charModel)
        {
            var mediator =
                MediateChild<TruckPointStaffCharMediator, TruckPointStaffCharModel>(TargetTransform, charModel);
            _childTruckPointStaffMediators[charModel] = mediator;
        }

        private void UnmediateTruckPointStaff(TruckPointStaffCharModel charModel)
        {
            if (_childTruckPointStaffMediators.TryGetValue(charModel, out var mediator))
            {
                UnmediateChild(mediator);
                _childTruckPointStaffMediators.Remove(charModel);
            }
            else
            {
                Debug.LogWarning($"Failed to unmediate staff char: pos = {charModel.CellCoords}");
            }
        }

        private void OnCustomerAdded(CustomerCharModel customerCharModel)
        {
            var mediator = MediateChild<CustomerCharMediator, CustomerCharModel>(TargetTransform, customerCharModel);
            _childCostumerMediators[customerCharModel] = mediator;
        }

        private void OnCustomerRemoved(CustomerCharModel customerCharModel)
        {
            if (_childCostumerMediators.TryGetValue(customerCharModel, out var mediator))
            {
                UnmediateChild(mediator);
                _childCostumerMediators.Remove(customerCharModel);
            }
            else
            {
                Debug.LogWarning($"Failed to unmediate customer: pos = {customerCharModel.CellCoords}");
            }
        }

        private void OnShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            if (shopObjectModel.ShopObjectType == ShopObjectType.TruckPoint)
            {
                var truckPointModel = (TruckPointModel)shopObjectModel;
                
                _truckPointsWatchList.Add(truckPointModel);
                
                HandleTruckPointStaffMediators(truckPointModel);
                
                SubscribeOnTruckPoint(truckPointModel);
            }
        }
    }
}