using System.Collections.Generic;
using System.Linq;
using Data;

namespace Model.Popups
{
    public class InteriorPopupViewModel : PopupViewModelBase
    {
        public readonly IReadOnlyList<InteriorPopupViewModelFloorItem> FloorItemViewModels;
        public readonly IReadOnlyList<InteriorPopupViewModelWallItem> WallItemViewModels;

        public InteriorPopupViewModel(
            IEnumerable<InteriorPopupViewModelFloorItem> floorItemViewModels,
            IEnumerable<InteriorPopupViewModelWallItem> wallItemViewModels)
        {
            FloorItemViewModels = floorItemViewModels.ToArray();
            WallItemViewModels = wallItemViewModels.ToArray();
        }

        public override PopupKey PopupKey => PopupKey.InteriorPopup;
    }

    public class InteriorPopupViewModelWallItem : InteriorPopupViewModelItemBase
    {
        public readonly WallType WallType;
        
        public InteriorPopupViewModelWallItem(int unlockLevel, bool isBought, bool isChosen, WallType wallType) 
            : base(unlockLevel, isBought, isChosen)
        {
            WallType = wallType;
        }
    }

    public class InteriorPopupViewModelFloorItem : InteriorPopupViewModelItemBase
    {
        public readonly FloorType FloorType;
        
        public InteriorPopupViewModelFloorItem(
            int unlockLevel, bool isBought, bool isChosen, FloorType floorType) 
            : base(unlockLevel, isBought, isChosen)
        {
            FloorType = floorType;
        }
    }
    
    public abstract class InteriorPopupViewModelItemBase
    {
        public readonly int UnlockLevel;

        public bool IsBought;
        public bool IsChosen;

        protected InteriorPopupViewModelItemBase(int unlockLevel, bool isBought, bool isChosen)
        {
            UnlockLevel = unlockLevel;
            IsBought = isBought;
            IsChosen = isChosen;
        }
    }
}