using Infra.Instance;
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

        public int GetCashDeskHireStaffCost()
        {
            var playerModelHolder = Instance.Get<IPlayerModelHolder>();
            var playerModel = playerModelHolder.PlayerModel;

            //var cashDesksAmount = playerModel.ShopModel.GetCashDeskModelsAmount();

            var cost = DefaultHireStaffMoneyCost;

            return cost <= playerModel.MoneyAmount ? cost : HireStaffWatchAdsCost;
        }
    }

    public interface IHireStaffCostProvider
    {
        public int GetTruckPointHireStaffCost(TruckPointModel truckPointModel);
        public int GetCashDeskHireStaffCost();
    }
}