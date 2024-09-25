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
        
        public void Execute(UIInteriorPopupItemClickedEvent e)
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var shopModel = playerModel.ShopModel;

            var itemViewModel = e.ItemViewModel;
            
            if (itemViewModel.IsBought == false 
                && itemViewModel.UnlockLevel <= playerModel.Level)
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
                    switch (itemViewModel)
                    {
                        case InteriorPopupWallItemViewModel wallItem:
                            playerModel.AddBoughtWall(wallItem.WallType);
                            break;
                        case InteriorPopupFloorItemViewModel floorItem:
                            playerModel.AddBoughtFloor(floorItem.FloorType);
                            break;
                    }
                }
            }
            
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
            }
        }
    }
}