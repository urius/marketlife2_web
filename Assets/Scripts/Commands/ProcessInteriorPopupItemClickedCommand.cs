using Data;
using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Model.Popups;
using Utils;

namespace Commands
{
    public class ProcessInteriorPopupItemClickedCommand : ICommand<UIInteriorPopupItemClickedEvent>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IPopupViewModelsHolder _popupViewModelsHolder = Instance.Get<IPopupViewModelsHolder>();
        
        public void Execute(UIInteriorPopupItemClickedEvent e)
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var shopModel = playerModel.ShopModel;
            var interiorPopupViewModel =
                (InteriorPopupViewModel)_popupViewModelsHolder.FindPopupByKey(PopupKey.InteriorPopup);

            var itemViewModel = e.ItemViewModel;
            if (itemViewModel.IsBought)
            {
                switch (itemViewModel)
                {
                    case InteriorPopupWallItemViewModel wallItem:
                        shopModel.SetWallsType(wallItem.WallType);
                        break;
                    case InteriorPopupFloorItemViewModel floorItem:
                        shopModel.SetFloorsType(floorItem.FloorType);
                        break;
                }
                
                interiorPopupViewModel.SetItemIsChosen(itemViewModel);
            }
            else if (itemViewModel.UnlockLevel <= playerModel.Level)
            {
                var level = playerModel.Level;
                
                var cost = itemViewModel switch
                {
                    InteriorPopupWallItemViewModel => InteriorCostHelper.GetWallCostForLevel(level),
                    InteriorPopupFloorItemViewModel => InteriorCostHelper.GetFloorCostForLevel(level),
                    _ => 999
                };

                if (playerModel.TrySpendMoney(cost))
                {
                    interiorPopupViewModel.SetItemBought(itemViewModel);
                }
            }
        }
    }
}