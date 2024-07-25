using Model.People;

namespace Events
{
    public struct CustomerStepFinishedEvent
    {
        public readonly CustomerCharModel CustomerModel;

        public CustomerStepFinishedEvent(CustomerCharModel customerModel)
        {
            CustomerModel = customerModel;
        }
    }
}