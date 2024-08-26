using Model.People;

namespace Events
{
    public struct RequestCashDeskStaffAcceptingPayAnimationEvent
    {
        public readonly CashDeskStaffModel StaffModel;
        public readonly bool RequestFinishAnimation;

        public RequestCashDeskStaffAcceptingPayAnimationEvent(CashDeskStaffModel staffModel, bool requestFinishAnimation = false)
        {
            StaffModel = staffModel;
            RequestFinishAnimation = requestFinishAnimation;
        }
    }
}