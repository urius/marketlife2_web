using Data;
using Holders;
using Infra.Instance;
using Model;
using Utils;

namespace View.UI.TopPanel
{
    public class UITopPanelLevelViewMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ICommonGameSettings _commonGameSettings = Instance.Get<ICommonGameSettings>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private UITopPanelLevelView _levelView;
        private PlayerModel _playerModel;
        private int _nextLevelMoneyAmount;

        protected override void MediateInternal()
        {
            _levelView = TargetTransform.GetComponent<UITopPanelLevelView>();

            _playerModel = _playerModelHolder.PlayerModel;
            
            UpdateNextLevelMoney();
            DisplayLevel();
            DisplayLevelProgress();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerModel.MoneyChanged += OnMoneyChanged;
            _playerModel.LevelChanged += OnLevelChanged;
        }

        private void Unsubscribe()
        {
            _playerModel.MoneyChanged -= OnMoneyChanged;
            _playerModel.LevelChanged -= OnLevelChanged;
        }

        private void OnMoneyChanged(int moneyAmount)
        {
            DisplayLevelProgress();
        }

        private void OnLevelChanged(int level)
        {
            UpdateNextLevelMoney();
            DisplayLevel();
            DisplayLevelProgress();
        }

        private void DisplayLevel()
        {
            var marketLevelStatus = _localizationProvider.GetLocale(Constants.LocalizationKeyMarketLevelPrefix + _playerModel.Level);
            
            _levelView.SetLevelStatusText(marketLevelStatus);
            _levelView.SetLevelText(_playerModel.Level.ToString());
        }

        private void DisplayLevelProgress()
        {
            if (_nextLevelMoneyAmount > 0)
            {
                var moneyAmountFormatted = FormattingHelper.ToCommaSeparatedNumber(_playerModel.MoneyAmount);
                var nextLevelMoneyAmountFormatted = FormattingHelper.ToCommaSeparatedNumber(_nextLevelMoneyAmount);
                
                _levelView.SetProgressText($"{moneyAmountFormatted} / {nextLevelMoneyAmountFormatted}");

                var progress = (float)_playerModel.MoneyAmount / _nextLevelMoneyAmount;
                _levelView.SetProgressBarRatio(progress > 1 ? 1 : progress);
            }
            else
            {
                _levelView.SetProgressText("max");
            }
        }

        private void UpdateNextLevelMoney()
        {
            var currentLevel = _commonGameSettings.GetLevelIndexByMoneyAmount(_playerModel.MoneyAmount);
            _nextLevelMoneyAmount = _commonGameSettings.GetLevelMoney(currentLevel + 1);
        }
    }
}