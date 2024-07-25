using Model.People;

namespace Events
{
    public struct CustomerFlyProductFromBasketAnimationFinishedEvent
    {
        public readonly CustomerCharModel TargetModel;

        public CustomerFlyProductFromBasketAnimationFinishedEvent(CustomerCharModel targetModel)
        {
            TargetModel = targetModel;
        }
    }
}