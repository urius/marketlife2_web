using Model.ShopObjects;

namespace Events
{
    public struct UpgradeTruckPointButtonClickedEvent
    {
        public readonly TruckPointModel TargetTruckPoint;

        public UpgradeTruckPointButtonClickedEvent(TruckPointModel targetTruckPoint)
        {
            TargetTruckPoint = targetTruckPoint;
        }
    }
}