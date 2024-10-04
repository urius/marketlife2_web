using System;

namespace Model
{
    public class PlayerMoneyEarnModifierModel
    {
        public event Action<int> TimeLeftChanged;
        
        private readonly int _multiplier;

        public PlayerMoneyEarnModifierModel(int multiplier, int timeLeftSeconds)
        {
            _multiplier = multiplier;
            TimeLeftSeconds = timeLeftSeconds;
        }

        public int TimeLeftSeconds { get; private set; }
        public int Multiplier => TimeLeftSeconds >= 0 ? _multiplier : 1;

        public void SetTimeLeft(int timeLeftSeconds)
        {
            TimeLeftSeconds = timeLeftSeconds;
            
            TimeLeftChanged?.Invoke(TimeLeftSeconds);
        }

        public int Apply(int initialDeltaMoney)
        {
            return initialDeltaMoney > 0 ? initialDeltaMoney * Multiplier : initialDeltaMoney;
        }
    }
}