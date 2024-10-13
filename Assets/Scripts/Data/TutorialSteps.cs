namespace Data
{
    public static class TutorialSteps
    {
        public static readonly TutorialStep[][] TutorialSequences =
        {
            new TutorialStep[]
            {
                TutorialStep.HowToMove,
                TutorialStep.BuildCashDesk,
                TutorialStep.BuildShelf,
                TutorialStep.BuildTruckPoint,
                TutorialStep.TakeProductFromTruckPoint,
                TutorialStep.PutProductToShelf,
                TutorialStep.TakeProductFromTruckPoint2,
                TutorialStep.PutProductToShelf2,
                TutorialStep.MoveToCashDesk,
                TutorialStep.HireCashDeskStaff,
                TutorialStep.UpgradeTruckPoint,
            },
            new TutorialStep[]
            {
                TutorialStep.OpenInteriorPopup,
                TutorialStep.OpenDressesPopup,
            },
            new TutorialStep[]
            {
                //TutorialStep.HireCashDeskStaff
            },
        };
    }

    public enum TutorialStep
    {
        HowToMove = 0,
        BuildCashDesk = 1,
        BuildShelf = 2,
        BuildTruckPoint = 3,
        TakeProductFromTruckPoint = 5,
        PutProductToShelf = 6,
        TakeProductFromTruckPoint2 = 7,
        PutProductToShelf2 = 8,
        MoveToCashDesk = 9,
        UpgradeShelf = 10,
        HireCashDeskStaff = 11,
        UpgradeTruckPoint = 12,
        OpenInteriorPopup = 13,
        OpenDressesPopup = 14,
    }
}