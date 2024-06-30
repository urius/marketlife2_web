using Model.Customers;

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