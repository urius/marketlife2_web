using Data;

namespace Model.Customers.States
{
    public abstract class CustomerStateBase
    {
        public abstract CustomerGlobalStateName StateName { get; }
    }
}