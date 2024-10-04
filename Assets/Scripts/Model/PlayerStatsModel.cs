namespace Model
{
    public class PlayerStatsModel
    {
        public PlayerStatsModel(long totalMoneyEarned)
        {
            TotalMoneyEarned = totalMoneyEarned;
        }

        public long TotalMoneyEarned { get; private set; }

        public void AddTotalMoneyEarned(int deltaMoney)
        {
            if (deltaMoney <= 0) return;

            TotalMoneyEarned += deltaMoney;
        }
    }
}