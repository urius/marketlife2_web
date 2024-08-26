using System;
using Data;
using Model.People;
using UnityEngine;

namespace Model.ShopObjects
{
    public class CashDeskModel : ShopObjectModelBase
    {
        public event Action MoneyAdded; 
        public event Action MoneyReset;
        
        public event Action<CashDeskStaffModel> StaffAdded;
        public event Action<CashDeskStaffModel> StaffRemoved;

        public CashDeskModel(Vector2Int cellCoords, CashDeskStaffModel staffModel = null) 
            : base(cellCoords)
        {
            CashDeskStaffModel = staffModel;
        }
        
        public CashDeskStaffModel CashDeskStaffModel { get; private set; }
        public int MoneyAmount { get; private set; }
        public bool HasCashMan => CashDeskStaffModel != null;
        public override ShopObjectType ShopObjectType => ShopObjectType.CashDesk;

        public void AddMoney(int moneyToAdd)
        {
            MoneyAmount += moneyToAdd;
            
            MoneyAdded?.Invoke();
        }
        
        public void ResetMoney()
        {
            MoneyAmount = 0;
            
            MoneyReset?.Invoke();
        }

        public void RemoveStaff()
        {
            SetStaffModel(null);
        }

        public void AddStaff(CashDeskStaffModel staffModel)
        {
            SetStaffModel(staffModel);
        }

        private void SetStaffModel(CashDeskStaffModel cashDeskStaffModel)
        {
            var prevStaffModel = CashDeskStaffModel;
            
            CashDeskStaffModel = cashDeskStaffModel;

            if (CashDeskStaffModel != null)
            {
                StaffAdded?.Invoke(CashDeskStaffModel);
            }
            else
            {
                StaffRemoved?.Invoke(prevStaffModel);
            }
        }
    }
}