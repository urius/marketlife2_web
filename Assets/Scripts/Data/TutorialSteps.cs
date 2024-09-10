namespace Data
{
    public static class TutorialSteps
    {
        public static readonly TutorialStep[][] TutorialSequences = new[]
        {
            new TutorialStep[]{
                TutorialStep.HowToMove,
                TutorialStep.BuildCashDesk,
                TutorialStep.BuildShelf,
                TutorialStep.BuildTruckPoint,
                TutorialStep.WaitProductsDeliver,
                TutorialStep.TakeProductFromTruckPoint,
                TutorialStep.MoveProductToShelf,
                TutorialStep.TakeProductFromTruckPoint2,
                TutorialStep.MoveProductToShelf2,
                TutorialStep.MoveToCashDesk,
            },
            new TutorialStep[]{
               // TutorialStep.UpgradeShelf
            },
            new TutorialStep[]{
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
        WaitProductsDeliver = 4,
        TakeProductFromTruckPoint = 5,
        MoveProductToShelf = 6,
        TakeProductFromTruckPoint2 = 7,
        MoveProductToShelf2 = 8,
        MoveToCashDesk = 9,
        UpgradeShelf = 10,
        HireCashDeskStaff = 11,
    }
}