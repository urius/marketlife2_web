using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Customers
{
    public class CustomersModel
    {
        private const int SpawnCooldownSeconds = 3;
        
        public event Action<CustomerCharModel> CustomerAdded;
        public event Action<CustomerCharModel> CustomerRemoved;
        
        private readonly List<CustomerCharModel> _customerModels = new(5);
        private readonly Dictionary<Vector2Int, CustomerCharModel> _customerModelByCell = new();
        
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

            SetOwnedCell(customer, customer.CellPosition);

            CustomerAdded?.Invoke(customer);
        }

        public void RemoveCustomer(CustomerCharModel customer)
        {
            TryRemoveOwnedCell(customer);
            
            _customerModels.Remove(customer);

            CustomerRemoved?.Invoke(customer);
        }

        public bool IsCellOwnedByCustomer(Vector2Int cellCoords)
        {
            return _customerModelByCell.ContainsKey(cellCoords);
        }
        
        public void SetOwnedCell(CustomerCharModel customer, Vector2Int cellCoords)
        {
            TryRemoveOwnedCell(customer);

            _customerModelByCell[cellCoords] = customer;
        }
        
        public bool TryRemoveOwnedCell(CustomerCharModel customer, Vector2Int cellPosition)
        {
            if (_customerModelByCell.TryGetValue(cellPosition, out var currentOwner)
                && currentOwner == customer)
            {
                _customerModelByCell.Remove(cellPosition);

                return true;
            }

            return false;
        }
        
        public bool TryRemoveOwnedCell(CustomerCharModel customer)
        {
            return TryRemoveOwnedCell(customer, customer.PreviousCellPosition)
                   || TryRemoveOwnedCell(customer, customer.CellPosition);
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

        public bool HaveCustomerOnCell(Vector2Int cell)
        {
            return _customerModelByCell.ContainsKey(cell);
        }
    }
}