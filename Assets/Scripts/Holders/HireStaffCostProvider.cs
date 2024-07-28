using Model.ShopObjects;

namespace Holders
{
    public class HireStaffCostProvider : IHireStaffCostProvider
    {
        public const int DefaultHireStaffMoneyCost = 5;
        public const int HireStaffWatchAdsCost = 0;

        public int GetTruckPointHireStaffCost(TruckPointModel truckPointModel)
        {
            return truckPointModel.GetHiredStaffAmount() switch
            {
                <= 0 => DefaultHireStaffMoneyCost,
                <= 1 => HireStaffWatchAdsCost,
                _ => -1
            };
        }
    }

    public interface IHireStaffCostProvider
    {
        public int GetTruckPointHireStaffCost(TruckPointModel truckPointModel);
    }
}