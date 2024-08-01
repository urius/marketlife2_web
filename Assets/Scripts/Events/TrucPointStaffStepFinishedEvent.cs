using Model.People;

namespace Events
{
    public struct TrucPointStaffStepFinishedEvent
    {
        public readonly TruckPointStaffCharModel CharModel;

        public TrucPointStaffStepFinishedEvent(TruckPointStaffCharModel charModel)
        {
            CharModel = charModel;
        }
    }
}