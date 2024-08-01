using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.People
{
    public class CustomersModel
    {
        private const int SpawnCooldownSeconds = 3;
        
        public event Action<CustomerCharModel> CustomerAdded;
        public event Action<CustomerCharModel> CustomerRemoved;
        
        private readonly List<CustomerCharModel> _customerModels = new(5);
        private readonly BotCharsOwnedCellModel _customersOwnedCellModel = new();
        
        public int MaxCustomersAmount
        {
            get => _customerModels.Capacity;
            set => _customerModels.Capacity = value;
        }
        public int SpawnCooldownSecondsLeft { get; private set; } = SpawnCooldownSeconds;
        public int CustomersAmount => _customerModels.Count;
        public IReadOnlyList<CustomerCharModel> Customers => _customerModels;

        public void AddCustomer(CustomerCharModel customer)
        {
            _customerModels.Add(customer);

            SetOwnedCell(customer, customer.CellCoords);

            CustomerAdded?.Invoke(customer);
        }

        public void RemoveCustomer(CustomerCharModel customer)
        {
            TryRemoveOwnedCell(customer);
            
            _customerModels.Remove(customer);

            CustomerRemoved?.Invoke(customer);
        }

        public bool HaveCustomerOnCell(Vector2Int cell)
        {
            return _customersOwnedCellModel.HaveCustomerOnCell(cell);
        }
        
        public void SetOwnedCell(CustomerCharModel customer, Vector2Int cellCoords)
        {
            _customersOwnedCellModel.SetOwnedCell(customer, cellCoords);
        }
        
        public bool TryRemoveOwnedCell(CustomerCharModel customer)
        {
            return _customersOwnedCellModel.TryRemoveOwnedCell(customer);
        }
        
        public void AdvanceSpawnCooldown()
        {
            if (SpawnCooldownSecondsLeft <= 0) return;
            
            SpawnCooldownSecondsLeft--;
        }

        public void ResetSpawnCooldown()
        {
            SpawnCooldownSecondsLeft = SpawnCooldownSeconds;
        }
    }
}