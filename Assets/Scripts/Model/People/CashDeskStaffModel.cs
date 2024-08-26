using UnityEngine;

namespace Model.People
{
    public class CashDeskStaffModel : StaffCharModelBase
    {
        public CashDeskStaffModel(Vector2Int cellCoords, int workSeconds, int workSecondsLeftSetting) 
            : base(cellCoords, workSeconds, workSecondsLeftSetting)
        {
        }
    }
}