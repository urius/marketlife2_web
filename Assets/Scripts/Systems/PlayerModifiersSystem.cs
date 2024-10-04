using Holders;
using Infra.Instance;
using Model;

namespace Systems
{
    public class PlayerModifiersSystem : ISystem
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private PlayerModel _playerModel;

        public void Start()
        {
            _playerModel = _playerModelHolder.PlayerModel;

            if (_playerModel.MoneyEarnModifier != null)
            {
                SubscribeOnUpdates();
            }

            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerModel.MoneyEarnModifierAdded += OnMoneyEarnModifierAdded;
        }

        private void Unsubscribe()
        {
            UnsubscribeFromUpdates();
            _playerModel.MoneyEarnModifierAdded -= OnMoneyEarnModifierAdded;
        }

        private void OnMoneyEarnModifierAdded(PlayerMoneyEarnModifierModel modifier)
        {
            SubscribeOnUpdates();
        }

        private void SubscribeOnUpdates()
        {
            UnsubscribeFromUpdates();
            _updatesProvider.GameplaySecondPassed += OnGameplaySecondPassed;
        }
        
        private void UnsubscribeFromUpdates()
        {
            _updatesProvider.GameplaySecondPassed -= OnGameplaySecondPassed;
        }

        private void OnGameplaySecondPassed()
        {
            var modifier = _playerModel.MoneyEarnModifier;
            if (modifier != null)
            {
                if (modifier.TimeLeftSeconds <= 0)
                {
                    _playerModel.RemoveMoneyEarnModifier();
                    
                    UnsubscribeFromUpdates();
                }
                else
                {
                    modifier.SetTimeLeft(modifier.TimeLeftSeconds - 1);
                }
            }
        }
    }
}