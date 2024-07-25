using System.Collections.Generic;
using Holders;
using Infra.Instance;
using Model.People;
using UnityEngine;

namespace View.Game.People
{
    public class CustomersMediator : MediatorBase
    {
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly Dictionary<CustomerCharModel, CustomerCharMediator> _childMediators = new();

        private CustomersModel _customersModel;

        protected override void MediateInternal()
        {
            _customersModel = _shopModelHolder.ShopModel.CustomersModel;

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
        }

        private void Unsubscribe()
        {
            _customersModel.CustomerAdded -= OnCustomerAdded;
            _customersModel.CustomerRemoved -= OnCustomerRemoved;
        }

        private void OnCustomerAdded(CustomerCharModel customerCharModel)
        {
            var mediator = MediateChild<CustomerCharMediator, CustomerCharModel>(TargetTransform, customerCharModel);
            _childMediators[customerCharModel] = mediator;
        }

        private void OnCustomerRemoved(CustomerCharModel customerCharModel)
        {
            if (_childMediators.TryGetValue(customerCharModel, out var mediator))
            {
                UnmediateChild(mediator);
                _childMediators.Remove(customerCharModel);
            }
            else
            {
                Debug.LogWarning($"Failed to unmediate customer: pos = {customerCharModel.CellPosition}");
            }
        }
    }
}