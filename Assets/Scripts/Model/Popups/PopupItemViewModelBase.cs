namespace Model.Popups
{
    public class PopupItemViewModelBase
    {
        public readonly int UnlockLevel;
        public readonly bool IsNew;

        public bool IsBought;
        public bool IsChosen;

        protected PopupItemViewModelBase(int unlockLevel, bool isBought, bool isChosen, bool isNew)
        {
            UnlockLevel = unlockLevel;
            IsBought = isBought;
            IsChosen = isChosen;
            IsNew = isNew;
        }
    }
}