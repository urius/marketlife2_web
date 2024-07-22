using Model.ShopObjects;

namespace Model.ViewModel
{
    public class BottomPanelUpgradeViewModel : BottomPanelViewModelBase
    {
        public readonly int UpgradeCost;
        public readonly ShopObjectModelBase TargetModel;

        public BottomPanelUpgradeViewModel(int upgradeCost, ShopObjectModelBase targetModel)
        {
            UpgradeCost = upgradeCost;
            TargetModel = targetModel;
        }
    }
}