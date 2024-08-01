using Model.People;

namespace Events
{
    public struct TruckPointStaffCharInitializedEvent
    {
        public readonly TruckPointStaffCharModel CharModel;

        public TruckPointStaffCharInitializedEvent(TruckPointStaffCharModel charModel)
        {
            CharModel = charModel;
        }
    }
}