using System;
using System.Collections.Generic;
using System.Linq;
using Data;

namespace Model.Popups
{
    public class InteriorPopupViewModel : PopupViewModelBase
    {
        public event Action<InteriorPopupItemViewModelBase> ItemBought;
        public event Action<InteriorPopupItemViewModelBase> ItemChosen;

        public readonly IReadOnlyList<InteriorPopupFloorItemViewModel> FloorItemViewModels;
        public readonly IReadOnlyList<InteriorPopupWallItemViewModel> WallItemViewModels;
        private readonly PlayerModel _playerModel;

        public InteriorPopupViewModel(
            IEnumerable<InteriorPopupFloorItemViewModel> floorItemViewModels,
            IEnumerable<InteriorPopupWallItemViewModel> wallItemViewModels,
            PlayerModel playerModel)
        {
            _playerModel = playerModel;
            
            FloorItemViewModels = floorItemViewModels.ToArray();
            WallItemViewModels = wallItemViewModels.ToArray();

            Subscribe();
        }

        public override PopupKey PopupKey => PopupKey.InteriorPopup;
        public override void Dispose()
        {
            Unsubscribe();
        }

        private void SetWallIsChosen(InteriorPopupItemViewModelBase itemViewModel)
        {
            foreach (var wallItemViewModel in WallItemViewModels)
            {
                wallItemViewModel.IsChosen = false;
            }

            itemViewModel.IsChosen = true;

            ItemChosen?.Invoke(itemViewModel);
        }

        private void SetFloorIsChosen(InteriorPopupItemViewModelBase itemViewModel)
        {
            foreach (var floorItemViewModel in FloorItemViewModels)
            {
                floorItemViewModel.IsChosen = false;
            }

            itemViewModel.IsChosen = true;
            
            ItemChosen?.Invoke(itemViewModel);
        }

        private void SetItemBought(InteriorPopupItemViewModelBase itemViewModel)
        {
            itemViewModel.IsBought = true;

            ItemBought?.Invoke(itemViewModel);
        }

        private void Subscribe()
        {
            _playerModel.WallBought += OnWallBought;
            _playerModel.FloorBought += OnFloorBought;
            _playerModel.ShopModel.WallsTypeUpdated += OnWallsTypeUpdated;
            _playerModel.ShopModel.FloorsTypeUpdated += OnFloorsTypeUpdated;
        }
        
        private void Unsubscribe()
        {
            _playerModel.WallBought -= OnWallBought;
            _playerModel.FloorBought -= OnFloorBought;
            _playerModel.ShopModel.WallsTypeUpdated -= OnWallsTypeUpdated;
            _playerModel.ShopModel.FloorsTypeUpdated -= OnFloorsTypeUpdated;
        }

        private void OnWallBought(WallType wallType)
        {
            var itemViewModel = WallItemViewModels.First(vm => vm.WallType == wallType);
            SetItemBought(itemViewModel);
        }

        private void OnFloorBought(FloorType floorType)
        {
            var itemViewModel = FloorItemViewModels.First(vm => vm.FloorType == floorType);
            SetItemBought(itemViewModel);
        }

        private void OnWallsTypeUpdated(WallType wallType)
        {
            var itemViewModel = WallItemViewModels.First(vm => vm.WallType == wallType);
            SetWallIsChosen(itemViewModel);
        }

        private void OnFloorsTypeUpdated(FloorType floorType)
        {
            var itemViewModel = FloorItemViewModels.First(vm => vm.FloorType == floorType);
            SetFloorIsChosen(itemViewModel);
        }
    }

    public class InteriorPopupWallItemViewModel : InteriorPopupItemViewModelBase
    {
        public readonly WallType WallType;
        
        public InteriorPopupWallItemViewModel(int unlockLevel, bool isBought, bool isChosen, WallType wallType) 
            : base(unlockLevel, isBought, isChosen)
        {
            WallType = wallType;
        }
    }

    public class InteriorPopupFloorItemViewModel : InteriorPopupItemViewModelBase
    {
        public readonly FloorType FloorType;
        
        public InteriorPopupFloorItemViewModel(
            int unlockLevel, bool isBought, bool isChosen, FloorType floorType) 
            : base(unlockLevel, isBought, isChosen)
        {
            FloorType = floorType;
        }
    }
    
    public abstract class InteriorPopupItemViewModelBase
    {
        public readonly int UnlockLevel;

        public bool IsBought;
        public bool IsChosen;

        protected InteriorPopupItemViewModelBase(int unlockLevel, bool isBought, bool isChosen)
        {
            UnlockLevel = unlockLevel;
            IsBought = isBought;
            IsChosen = isChosen;
        }
    }
}