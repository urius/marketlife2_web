using Model.ShopObjects;

namespace Model.ViewModel
{
    public class TruckPointPanelViewModel : BottomPanelViewModelBase
    {
        public readonly int UpgradeCost;
        public readonly TruckPointModel TargetModel;

        public TruckPointPanelViewModel(int upgradeCost, TruckPointModel targetModel)
        {
            UpgradeCost = upgradeCost;
            TargetModel = targetModel;
        }
    }
}