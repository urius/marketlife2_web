using Data;
using Events;
using Extensions;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Model.Popups;
using Tools.AudioManager;
using Utils;

namespace Commands
{
    public class ProcessInteriorPopupItemClickedCommand : ICommand<UIInteriorPopupItemClickedEvent>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        public void Execute(UIInteriorPopupItemClickedEvent e)
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var shopModel = playerModel.ShopModel;

            var itemViewModel = e.ItemViewModel;
            
            if (itemViewModel.IsBought == false 
                && itemViewModel.UnlockLevel <= playerModel.Level)
            {
                var itemLevel = itemViewModel.UnlockLevel;
                
                var cost = itemViewModel switch
                {
                    InteriorPopupWallItemViewModel => CostHelper.GetWallCostForLevel(itemLevel),
                    InteriorPopupFloorItemViewModel => CostHelper.GetFloorCostForLevel(itemLevel),
                    _ => 999
                };

                if (playerModel.TrySpendMoney(cost))
                {
                    _audioPlayer.PlaySound(SoundIdKey.CashSound_2);
                    
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