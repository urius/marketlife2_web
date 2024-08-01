using Model.People;

namespace Events
{
    public struct StaffTakeBoxFromTruckAnimationFinishedEvent
    {
        public readonly TruckPointStaffCharModel CharModel;

        public StaffTakeBoxFromTruckAnimationFinishedEvent(TruckPointStaffCharModel charModel)
        {
            CharModel = charModel;
        }
    }
}