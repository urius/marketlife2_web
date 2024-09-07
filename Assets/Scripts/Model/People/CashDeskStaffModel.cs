using UnityEngine;

namespace Model.People
{
    public class CashDeskStaffModel : StaffCharModelBase
    {
        public CashDeskStaffModel(Vector2Int cellCoords, int workSeconds) 
            : base(cellCoords, workSeconds)
        {
        }
    }
}