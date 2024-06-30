using Model.Customers;

namespace Events
{
    public struct CustomerTakeProductAnimationFinishedEvent
    {
        public readonly CustomerCharModel TargetModel;

        public CustomerTakeProductAnimationFinishedEvent(CustomerCharModel targetModel)
        {
            TargetModel = targetModel;
        }
    }
}