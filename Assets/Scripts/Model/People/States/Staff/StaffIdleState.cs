using Data;

namespace Model.People.States.Staff
{
    public class StaffIdleState : BotCharStateBase
    {
        public static StaffIdleState Instance = new StaffIdleState();

        private StaffIdleState()
        {
        }
        
        public override ShopCharStateName StateName => ShopCharStateName.TpStaffIdle;
    }
}