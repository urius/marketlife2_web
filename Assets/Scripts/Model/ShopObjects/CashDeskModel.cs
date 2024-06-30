using System;
using Data;
using UnityEngine;

namespace Model.ShopObjects
{
    public class CashDeskModel : ShopObjectModelBase
    {
        public event Action MoneyAdded; 
        
        public CashDeskModel(Vector2Int cellCoords) 
            : base(cellCoords)
        {
        }

        public int MoneyAmount { get; private set; }
        public bool HasCashMan => false;
        public override ShopObjectType ShopObjectType => ShopObjectType.CashDesk;

        public void AddMoney(int moneyToAdd)
        {
            MoneyAmount += moneyToAdd;
            
            MoneyAdded?.Invoke();
        }
    }
}