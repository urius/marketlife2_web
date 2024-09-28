using System;
using Data;
using Extensions;
using Holders;
using Infra.Instance;
using Model;
using Tools.AudioManager;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace View.UI.TopPanel
{
    public class UITopPanelMoneyViewMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();

        private readonly MoneyAnimationContext _moneyAnimationContext = new ();
        private readonly Color[] _blinkColors = { Color.white, Color.red };
        
        private UITopPanelMoneyView _moneyView;
        private PlayerModel _playerModel;
        private int _currentMoneyDisplayed = -1;
        private int _blinksCounter;
        private int _blinkMoneyCooldown;
        private int _currentColorIndex;
        private float _moneyIconXPositionOffsetPercent = 0.5f;
        private int _moneyIconXPositionDirection = 0;

        protected override void MediateInternal()
        {
            _moneyView = TargetTransform.GetComponent<UITopPanelMoneyView>();

            _playerModel = _playerModelHolder.PlayerModel;

            Subscribe();

            DisplayMoney(_playerModel.MoneyAmount);
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            _moneyView = null;
        }

        private void Subscribe()
        {
            _playerModel.MoneyChanged += OnMoneyChanged;
            _playerModel.InsufficientFunds += OnInsufficientFundsSignal;
        }

        private void Unsubscribe()
        {
            _playerModel.MoneyChanged -= OnMoneyChanged;
            _playerModel.InsufficientFunds -= OnInsufficientFundsSignal;
            
            _updatesProvider.GameplayFixedUpdate -= OnMoneyAnimationGameplayFixedUpdate;
        }

        private void OnInsufficientFundsSignal(int _)
        {
            _updatesProvider.GameplayFixedUpdate -= OnBlinkMoneyGameplayFixedUpdate;
            _updatesProvider.GameplayFixedUpdate += OnBlinkMoneyGameplayFixedUpdate;
            
            _blinksCounter = 0;
            _currentColorIndex = 0;
            _blinkMoneyCooldown = 0;
            _moneyIconXPositionOffsetPercent = 0.5f;
            _moneyIconXPositionDirection = Random.value <= 0.5f ? 1 : -1;
        }

        private void OnBlinkMoneyGameplayFixedUpdate()
        {
            _blinkMoneyCooldown--;

            _moneyIconXPositionOffsetPercent += Time.fixedDeltaTime * _moneyIconXPositionDirection * 10;
            var offset = Mathf.Lerp(-5, 5, _moneyIconXPositionOffsetPercent);
            _moneyView.SetMoneyIconXOffset(offset);

            if ((_moneyIconXPositionOffsetPercent > 1 && _moneyIconXPositionDirection > 0)
                || (_moneyIconXPositionOffsetPercent <= 0 && _moneyIconXPositionDirection < 0))
            {
                _moneyIconXPositionDirection *= -1;
            }

            if (_blinkMoneyCooldown <= 0)
            {
                _blinksCounter++;
                _blinkMoneyCooldown = 5;
                _currentColorIndex = 1 - _currentColorIndex;

                _moneyView.SetTextColor(_blinkColors[_currentColorIndex]);
            }

            if (_blinksCounter >= 10)
            {
                _moneyView.SetDefaultTextColor();
                _moneyView.ResetMoneyIconPosition();
                _updatesProvider.GameplayFixedUpdate -= OnBlinkMoneyGameplayFixedUpdate;
            }
        }

        private void OnMoneyChanged(int _)
        {
            if (Math.Abs(_playerModel.MoneyAmount - _currentMoneyDisplayed) <= 1)
            {
                DisplayMoney(_playerModel.MoneyAmount);
            }
            else
            {
                if (_moneyAnimationContext.Progress < 1) return;
                
                _moneyAnimationContext.Reset(_currentMoneyDisplayed);

                _updatesProvider.GameplayFixedUpdate -= OnMoneyAnimationGameplayFixedUpdate;
                _updatesProvider.GameplayFixedUpdate += OnMoneyAnimationGameplayFixedUpdate;
            }
        }

        private void OnMoneyAnimationGameplayFixedUpdate()
        {
            _moneyAnimationContext.Progress += 3 * Time.fixedDeltaTime;

            if (_moneyAnimationContext.Progress >= 1)
            {
                _updatesProvider.GameplayFixedUpdate -= OnMoneyAnimationGameplayFixedUpdate;
            }

            var moneyValue = (int)Mathf.Lerp(_moneyAnimationContext.StartValue, _playerModel.MoneyAmount,
                _moneyAnimationContext.Progress);
            DisplayMoney(moneyValue);

            var isPositiveDelta = _playerModel.MoneyAmount > _moneyAnimationContext.StartValue;
            var moneyColor = Color.Lerp(isPositiveDelta ? Color.green : Color.yellow, _moneyView.TextDefaultColor, _moneyAnimationContext.Progress);
            _moneyView.SetTextColor(moneyColor);
        }

        private void DisplayMoney(int moneyAmount)
        {
            if (moneyAmount > _currentMoneyDisplayed && _currentMoneyDisplayed >= 0)
            {
                _audioPlayer.PlaySound(SoundIdKey.MoneyTick);
            }

            _currentMoneyDisplayed = moneyAmount;
            
            var moneyAmountFormatted = FormattingHelper.ToCommaSeparatedNumber(_currentMoneyDisplayed); 

            _moneyView.SetMoneyText(moneyAmountFormatted);
        }
        
        private class MoneyAnimationContext
        {
            public int StartValue = -1;
            public float Progress = 1;

            public void Reset(int startValue)
            {
                StartValue = startValue;
                Progress = 0;
            }
        }
    }
}