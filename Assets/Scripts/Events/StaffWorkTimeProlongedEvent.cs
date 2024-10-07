using Model.People;

namespace Events
{
    public struct StaffWorkTimeProlongedEvent
    {
        public readonly StaffCharModelBase StaffCharModel;

        public StaffWorkTimeProlongedEvent(StaffCharModelBase staffCharModel)
        {
            StaffCharModel = staffCharModel;
        }
    }
}