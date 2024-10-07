using System;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerStatsDto
    {
        public long TotalMoneyEarned;

        public PlayerStatsDto(long totalMoneyEarned)
        {
            TotalMoneyEarned = totalMoneyEarned;
        }
    }
}