using Model.People;

namespace Events
{
    public struct TruckPointStaffStepFinishedEvent
    {
        public readonly TruckPointStaffCharModel CharModel;

        public TruckPointStaffStepFinishedEvent(TruckPointStaffCharModel charModel)
        {
            CharModel = charModel;
        }
    }
}