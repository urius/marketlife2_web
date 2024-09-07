using Infra.Instance;
using Model.People;
using Model.ShopObjects;

namespace Holders
{
    public class HireStaffCostProvider : IHireStaffCostProvider
    {
        public const int DefaultHireStaffMoneyCost = 5;
        public const int HireStaffWatchAdsCost = 0;

        public int GetTruckPointHireStaffCost(TruckPointModel truckPointModel)
        {
            return GetHireStaffCost(truckPointModel.StaffCharModel);
        }

        public int GetCashDeskHireStaffCost(CashDeskModel cashDeskModel)
        {
            return GetHireStaffCost(cashDeskModel.CashDeskStaffModel);
        }

        private static int GetHireStaffCost(StaffCharModelBase staffCharModel)
        {
            var playerModelHolder = Instance.Get<IPlayerModelHolder>();
            var playerModel = playerModelHolder.PlayerModel;
            
            if (staffCharModel == null || staffCharModel.WorkSecondsLeft <= playerModel.StaffWorkTimeSeconds)
            {
                return DefaultHireStaffMoneyCost;
            }
            else
            {
                return HireStaffWatchAdsCost;
            }
        }
    }

    public interface IHireStaffCostProvider
    {
        public int GetTruckPointHireStaffCost(TruckPointModel truckPointModel);
        public int GetCashDeskHireStaffCost(CashDeskModel cashDeskModel);
    }
}