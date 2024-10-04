using Holders;
using Infra.Instance;
using Model;
using Utils;

namespace View.UI.TopPanel
{
    public class UIMoneyEarnMultiplierViewMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private UIMoneyEarnMultiplierModifierView _view;
        private PlayerModel _playerModel;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            
            _view = TargetTransform.GetComponent<UIMoneyEarnMultiplierModifierView>();
            UpdateView();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            _view = null;
        }
        
        private void Subscribe()
        {
            _playerModel.MoneyEarnModifierAdded += OnMoneyEarnModifierAdded;
            _playerModel.MoneyEarnModifierRemoved += OnMoneyEarnModifierRemoved;
        }

        private void Unsubscribe()
        {
            UnsubscribeFromUpdates();
            _playerModel.MoneyEarnModifierAdded -= OnMoneyEarnModifierAdded;
            _playerModel.MoneyEarnModifierRemoved -= OnMoneyEarnModifierRemoved;
        }

        private void OnMoneyEarnModifierAdded(PlayerMoneyEarnModifierModel model)
        {
            UpdateView();
            SubscribeOnUpdates();
        }

        private void OnMoneyEarnModifierRemoved(PlayerMoneyEarnModifierModel model)
        {
            UnsubscribeFromUpdates();
            UpdateView();
        }

        private void SubscribeOnUpdates()
        {
            _updatesProvider.GameplaySecondPassed += OnGameplaySecondPassed;
        }

        private void UnsubscribeFromUpdates()
        {
            _updatesProvider.GameplaySecondPassed -= OnGameplaySecondPassed;
        }

        private void OnGameplaySecondPassed()
        {
            UpdateTimeLeftText(_playerModel.MoneyEarnModifier);
        }

        private void UpdateView()
        {
            var modifier = _playerModel.MoneyEarnModifier;
            _view.gameObject.SetActive(modifier != null);

            if (modifier == null) return;
            
            UpdateMultiplierText(modifier);
            UpdateTimeLeftText(modifier);
        }

        private void UpdateTimeLeftText(PlayerMoneyEarnModifierModel modifier)
        {
            if(modifier == null) return;
            
            var timeLeftText = FormattingHelper.ToTimeFormatMinSec(modifier.TimeLeftSeconds);
            _view.SetTimeLeftText(timeLeftText);
        }

        private void UpdateMultiplierText(PlayerMoneyEarnModifierModel modifier)
        {
            _view.SetMultiplierText($"{modifier.Multiplier}x");
        }
    }
}