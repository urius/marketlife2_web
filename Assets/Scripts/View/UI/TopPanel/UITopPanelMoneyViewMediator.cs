using System;
using Holders;
using Infra.Instance;
using Model;
using UnityEngine;

namespace View.UI.TopPanel
{
    public class UITopPanelMoneyViewMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();

        private readonly MoneyAnimationContext _moneyAnimationContext = new ();
        
        private UITopPanelMoneyView _moneyView;
        private PlayerModel _playerModel;
        private int _currentMoneyDisplayed;

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
        }

        private void Unsubscribe()
        {
            _playerModel.MoneyChanged -= OnMoneyChanged;
            
            _updatesProvider.GameplayFixedUpdate -= OnMoneyAnimationGameplayFixedUpdate;
        }

        private void OnMoneyChanged(int _)
        {
            if (Math.Abs(_playerModel.MoneyAmount - _currentMoneyDisplayed) <= 3)
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
            _moneyAnimationContext.Progress += Time.fixedDeltaTime;

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
            _currentMoneyDisplayed = moneyAmount;
            
            _moneyView.SetMoneyText(_currentMoneyDisplayed.ToString());
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