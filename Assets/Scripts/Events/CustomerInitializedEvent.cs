using Model.Customers;

namespace Events
{
    public struct CustomerInitializedEvent
    {
        public readonly CustomerCharModel CustomerModel;

        public CustomerInitializedEvent(CustomerCharModel customerModel)
        {
            CustomerModel = customerModel;
        }
    }
}