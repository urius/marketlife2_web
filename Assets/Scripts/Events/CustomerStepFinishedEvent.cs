using Model.Customers;

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