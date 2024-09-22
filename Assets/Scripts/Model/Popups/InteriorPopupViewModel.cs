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

        public InteriorPopupViewModel(
            IEnumerable<InteriorPopupFloorItemViewModel> floorItemViewModels,
            IEnumerable<InteriorPopupWallItemViewModel> wallItemViewModels)
        {
            FloorItemViewModels = floorItemViewModels.ToArray();
            WallItemViewModels = wallItemViewModels.ToArray();
        }

        public override PopupKey PopupKey => PopupKey.InteriorPopup;

        public void SetItemBought(InteriorPopupItemViewModelBase itemViewModel)
        {
            itemViewModel.IsBought = true;

            ItemBought?.Invoke(itemViewModel);
        }

        public void SetItemIsChosen(InteriorPopupItemViewModelBase itemViewModel)
        {
            if (itemViewModel is InteriorPopupWallItemViewModel)
            {
                    foreach (var floorItemViewModel in WallItemViewModels)
                    {
                        floorItemViewModel.IsChosen = false;
                    }
            }
            else if (itemViewModel is InteriorPopupFloorItemViewModel)
            {
                foreach (var floorItemViewModel in FloorItemViewModels)
                {
                    floorItemViewModel.IsChosen = false;
                }
            }

            itemViewModel.IsChosen = true;
            
            ItemChosen?.Invoke(itemViewModel);
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