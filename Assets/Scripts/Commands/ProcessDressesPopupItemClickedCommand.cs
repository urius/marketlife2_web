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
    public class ProcessDressesPopupItemClickedCommand : ICommand<UIDressesPopupItemClickedEvent>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        public void Execute(UIDressesPopupItemClickedEvent e)
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var dressesModel = _playerModelHolder.PlayerCharModel.DressesModel;
            
            var itemViewModel = e.ItemViewModel;
            
            if (itemViewModel.IsBought == false 
                && itemViewModel.UnlockLevel <= playerModel.Level)
            {
                var itemLevel = itemViewModel.UnlockLevel;

                var cost = itemViewModel.TargetTabType switch
                {
                    PlayerDressesPopupTabType.TopDresses => CostHelper.GetTopDressCostForLevel(itemLevel),
                    PlayerDressesPopupTabType.BottomDresses => CostHelper.GetBottomDressCostForLevel(itemLevel),
                    PlayerDressesPopupTabType.Hairs => CostHelper.GetHairCostForLevel(itemLevel),
                    PlayerDressesPopupTabType.Glasses => CostHelper.GetGlassesCostForLevel(itemLevel),
                    _ => 999
                };

                if (playerModel.TrySpendMoney(cost))
                {
                    _audioPlayer.PlaySound(SoundIdKey.CashSound_2);
                    
                    switch (itemViewModel.TargetTabType)
                    {
                        case PlayerDressesPopupTabType.TopDresses:
                            dressesModel.AddBoughtTopDress(itemViewModel.PrimarySpriteType);
                            break;
                        case PlayerDressesPopupTabType.BottomDresses:
                            dressesModel.AddBoughtBottomDress(itemViewModel.PrimarySpriteType);
                            break;
                        case PlayerDressesPopupTabType.Hairs:
                            dressesModel.AddBoughtHair(itemViewModel.PrimarySpriteType);
                            break;
                        case PlayerDressesPopupTabType.Glasses:
                            dressesModel.AddBoughtGlasses(itemViewModel.PrimarySpriteType);
                            break;
                    }
                }
            }
            
            if (itemViewModel.IsBought)
            {
                switch (itemViewModel.TargetTabType)
                {
                    case PlayerDressesPopupTabType.TopDresses:
                        dressesModel.TopDressType = itemViewModel.PrimarySpriteType;
                        break;
                    case PlayerDressesPopupTabType.BottomDresses:
                        dressesModel.BottomDressType = itemViewModel.PrimarySpriteType;
                        break;
                    case PlayerDressesPopupTabType.Hairs:
                        dressesModel.HairType = itemViewModel.PrimarySpriteType;
                        break;
                    case PlayerDressesPopupTabType.Glasses:
                        dressesModel.GlassesType = itemViewModel.PrimarySpriteType;
                        break;
                }
            }
        }
    }
}