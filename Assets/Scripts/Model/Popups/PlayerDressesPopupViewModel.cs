using System;
using System.Collections.Generic;
using System.Linq;
using Data;

namespace Model.Popups
{
    public class PlayerDressesPopupViewModel : PopupViewModelBase
    {
        public event Action<DressesPopupItemViewModel> ItemBought;
        public event Action<DressesPopupItemViewModel> ItemChosen;
        
        public event Action<PlayerDressesPopupTabType> TabChanged;
        
        public readonly IReadOnlyList<DressesPopupItemViewModel> TopDressItemViewModels;
        public readonly IReadOnlyList<DressesPopupItemViewModel> BottomDressItemViewModels;
        public readonly IReadOnlyList<DressesPopupItemViewModel> HairItemViewModels;
        public readonly IReadOnlyList<DressesPopupItemViewModel> GlassesItemViewModels;

        public readonly PlayerDressesPopupTabType[] TabTypes =
        {
            PlayerDressesPopupTabType.TopDresses, PlayerDressesPopupTabType.BottomDresses,
            PlayerDressesPopupTabType.Hairs, PlayerDressesPopupTabType.Glasses
        };
        
        private readonly PlayerDressesModel _playerDressesModel;

        public PlayerDressesPopupViewModel(IReadOnlyList<DressesPopupItemViewModel> topDressItemViewModels,
            IReadOnlyList<DressesPopupItemViewModel> bottomDressItemViewModels,
            IReadOnlyList<DressesPopupItemViewModel> hairItemViewModels,
            IReadOnlyList<DressesPopupItemViewModel> glassesItemViewModels,
            PlayerDressesModel playerDressesModel)
        {
            TopDressItemViewModels = topDressItemViewModels;
            BottomDressItemViewModels = bottomDressItemViewModels;
            HairItemViewModels = hairItemViewModels;
            GlassesItemViewModels = glassesItemViewModels;
            
            _playerDressesModel = playerDressesModel;

            Subscribe();
        }

        public override PopupKey PopupKey => PopupKey.PlayerDressesPopup;

        public PlayerDressesPopupTabType CurrentTabType { get; private set; }


        public void SetTab(PlayerDressesPopupTabType tab)
        {
            if (CurrentTabType == tab) return;

            CurrentTabType = tab;

            TabChanged?.Invoke(tab);
        }

        public override void Dispose()
        {
            Unsubscribe();
        }

        public void NotifyTabClicked(int tabIndex)
        {
            SetTab(TabTypes[tabIndex]);
        }

        private void Subscribe()
        {
            _playerDressesModel.TopDressBought += OnTopDressBought;
            _playerDressesModel.BottomDressBought += OnBottomDressBought;
            _playerDressesModel.HairBought += OnHairBought;
            _playerDressesModel.GlassesBought += OnGlassesBought;
            _playerDressesModel.TopDressTypeChanged += OnTopDressTypeChanged;
            _playerDressesModel.BottomDressTypeChanged += OnBottomDressTypeChanged;
            _playerDressesModel.HairTypeChanged += OnHairTypeChanged;
            _playerDressesModel.GlassesTypeChanged += OnGlassesTypeChanged;
        }

        private void Unsubscribe()
        {
            _playerDressesModel.TopDressBought -= OnTopDressBought;
            _playerDressesModel.BottomDressBought -= OnBottomDressBought;
            _playerDressesModel.HairBought -= OnHairBought;
            _playerDressesModel.GlassesBought -= OnGlassesBought;
            _playerDressesModel.TopDressTypeChanged -= OnTopDressTypeChanged;
            _playerDressesModel.BottomDressTypeChanged -= OnBottomDressTypeChanged;
            _playerDressesModel.HairTypeChanged -= OnHairTypeChanged;
            _playerDressesModel.GlassesTypeChanged -= OnGlassesTypeChanged;
        }

        private void OnTopDressBought(ManSpriteType spriteType)
        {
            var itemViewModel = TopDressItemViewModels.First(vm => vm.PrimarySpriteType == spriteType);
            SetItemBought(itemViewModel);
        }

        private void OnBottomDressBought(ManSpriteType spriteType)
        {
            var itemViewModel = BottomDressItemViewModels.First(vm => vm.PrimarySpriteType == spriteType);
            SetItemBought(itemViewModel);
        }

        private void OnHairBought(ManSpriteType spriteType)
        {
            var itemViewModel = HairItemViewModels.First(vm => vm.PrimarySpriteType == spriteType);
            SetItemBought(itemViewModel);
        }

        private void OnGlassesBought(ManSpriteType spriteType)
        {
            var itemViewModel = GlassesItemViewModels.First(vm => vm.PrimarySpriteType == spriteType);
            SetItemBought(itemViewModel);
        }

        private void OnTopDressTypeChanged(ManSpriteType spriteType)
        {
            SetItemIsChosen(TopDressItemViewModels, spriteType);
        }

        private void OnBottomDressTypeChanged(ManSpriteType spriteType)
        {
            SetItemIsChosen(BottomDressItemViewModels, spriteType);
        }

        private void OnHairTypeChanged(ManSpriteType spriteType)
        {
            SetItemIsChosen(HairItemViewModels, spriteType);
        }

        private void OnGlassesTypeChanged(ManSpriteType spriteType)
        {
            SetItemIsChosen(GlassesItemViewModels, spriteType);
        }

        private void SetItemIsChosen(IReadOnlyList<DressesPopupItemViewModel> items, ManSpriteType spriteType)
        {
            foreach (var floorItemViewModel in items)
            {
                floorItemViewModel.IsChosen = false;
            }

            var itemViewModel = items.First(i => i.PrimarySpriteType == spriteType);

            itemViewModel.IsChosen = true;
            
            ItemChosen?.Invoke(itemViewModel);
        }

        private void SetItemBought(DressesPopupItemViewModel itemViewModel)
        {
            itemViewModel.IsBought = true;

            ItemBought?.Invoke(itemViewModel);
        }
    }

    public class DressesPopupItemViewModel : PopupItemViewModelBase
    {
        public readonly PlayerDressesPopupTabType TargetTabType;
        public readonly ManSpriteType PrimarySpriteType;
        public readonly ManSpriteType SecondarySpriteType;

        public DressesPopupItemViewModel(
            PlayerDressesPopupTabType targetTabType,
            ManSpriteType spriteType,
            ManSpriteType secondarySpriteType,
            int unlockLevel, bool isBought, bool isChosen, bool isNew)
            : base(unlockLevel, isBought, isChosen, isNew)
        {
            TargetTabType = targetTabType;
            PrimarySpriteType = spriteType;
            SecondarySpriteType = secondarySpriteType;
        }
    }

    public enum PlayerDressesPopupTabType
    {
        TopDresses,
        BottomDresses,
        Hairs,
        Glasses,
    }
}