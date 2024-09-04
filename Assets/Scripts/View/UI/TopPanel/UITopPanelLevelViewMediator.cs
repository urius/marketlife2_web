using System;
using Cysharp.Threading.Tasks;
using Data;
using Holders;
using Infra.Instance;
using Model;
using Utils;

namespace View.UI.TopPanel
{
    public class UITopPanelLevelViewMediator : MediatorBase
    {
        private const float LevelProgressFillSpeed = 0.01f;
        
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ICommonGameSettings _commonGameSettings = Instance.Get<ICommonGameSettings>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private UITopPanelLevelView _levelView;
        private PlayerModel _playerModel;
        private int _nextLevelMoneyAmount;
        private float _currentDisplayedLevelProgress;
        private float _targetDisplayedLevelProgress;

        private float CurrentLevelProgress => (float)_playerModel.MoneyAmount / _nextLevelMoneyAmount;
        
        protected override void MediateInternal()
        {
            _levelView = TargetTransform.GetComponent<UITopPanelLevelView>();

            _playerModel = _playerModelHolder.PlayerModel;

            UpdateVisibility();
            
            UpdateNextLevelMoney();
            DisplayLevel();
            DisplayLevelProgress(forceProgressBar: true);

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
            _playerModel.IsLevelProcessingActiveFlagUpdated += OnIsLevelProcessingActiveFlagUpdated;
        }

        private void Unsubscribe()
        {
            _playerModel.MoneyChanged -= OnMoneyChanged;
            _playerModel.LevelChanged -= OnLevelChanged;
            _playerModel.IsLevelProcessingActiveFlagUpdated -= OnIsLevelProcessingActiveFlagUpdated;
            _updatesProvider.GameplayFixedUpdate -= OnLevelProgressFixedUpdate;
        }

        private void OnIsLevelProcessingActiveFlagUpdated(bool isActive)
        {
            UpdateVisibility();
            
            UpdateNextLevelMoney();
            DisplayLevel();
            DisplayLevelProgress(forceProgressBar: true);
        }

        private void UpdateVisibility()
        {
            _levelView.SetVisibility(_playerModel.IsLevelProcessingActive);
        }

        private void OnMoneyChanged(int moneyAmount)
        {
            DisplayLevelProgress(forceProgressBar: false);
        }

        private void OnLevelChanged(int level)
        {
            ProcessLevelChanged().Forget();
        }

        private async UniTaskVoid ProcessLevelChanged()
        {
            UpdateNextLevelMoney();
            DisplayLevel();
            SetLevelProgressBarRatio(0f);
            DisplayLevelProgress(forceProgressBar: false);

            await AnimateNewLevelStar();
        }

        private async UniTask AnimateNewLevelStar()
        {
            InstantiatePrefab(PrefabKey.VFXStars, _levelView.transform);

            await LeanTweenHelper.BounceYAsync(_levelView.StarTransform, deltaY: -25, duration1: 0.5f, duration2: 1f);
        }

        private void DisplayLevel()
        {
            var marketLevelStatus = _localizationProvider.GetLocale(Constants.LocalizationKeyMarketLevelPrefix + _playerModel.Level);
            
            _levelView.SetLevelStatusText(marketLevelStatus);
            _levelView.SetLevelText(_playerModel.Level.ToString());
        }

        private void DisplayLevelProgress(bool forceProgressBar)
        {
            if (_nextLevelMoneyAmount > 0)
            {
                var moneyAmountFormatted = FormattingHelper.ToCommaSeparatedNumber(_playerModel.MoneyAmount);
                var nextLevelMoneyAmountFormatted = FormattingHelper.ToCommaSeparatedNumber(_nextLevelMoneyAmount);
                
                _levelView.SetProgressText($"{moneyAmountFormatted} / {nextLevelMoneyAmountFormatted}");

                _targetDisplayedLevelProgress = CurrentLevelProgress;

                _updatesProvider.GameplayFixedUpdate -= OnLevelProgressFixedUpdate;
                
                if (forceProgressBar)
                {
                    SetLevelProgressBarRatio(_targetDisplayedLevelProgress);
                }
                else
                {
                    _updatesProvider.GameplayFixedUpdate += OnLevelProgressFixedUpdate;
                }
            }
            else
            {
                _levelView.SetProgressText("max");
            }
        }

        private void OnLevelProgressFixedUpdate()
        {
            var deltaProgress = _targetDisplayedLevelProgress - _currentDisplayedLevelProgress;
            if (Math.Abs(deltaProgress) < 0.005f)
            {
                SetLevelProgressBarRatio(_targetDisplayedLevelProgress);
                
                _updatesProvider.GameplayFixedUpdate -= OnLevelProgressFixedUpdate;
                return;
            }

            var newProgress = _currentDisplayedLevelProgress + LevelProgressFillSpeed * (deltaProgress > 0 ? 1 : -1);
            SetLevelProgressBarRatio(newProgress);
        }

        private void SetLevelProgressBarRatio(float progress)
        {
            _currentDisplayedLevelProgress = progress;
            _levelView.SetProgressBarRatio(progress > 1 ? 1 : progress);
        }

        private void UpdateNextLevelMoney()
        {
            var currentLevel = _commonGameSettings.GetLevelIndexByMoneyAmount(_playerModel.MoneyAmount);
            _nextLevelMoneyAmount = _commonGameSettings.GetLevelTargetMoney(currentLevel + 1);
        }
    }
}