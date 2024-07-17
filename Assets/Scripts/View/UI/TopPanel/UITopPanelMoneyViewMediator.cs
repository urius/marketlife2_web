using Holders;
using Infra.Instance;
using Model;

namespace View.UI.TopPanel
{
    public class UITopPanelMoneyViewMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        
        private UITopPanelMoneyView _moneyView;
        private PlayerModel _playerModel;

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
        }

        private void OnMoneyChanged(int moneyAmount)
        {
            DisplayMoney(moneyAmount);
        }

        private void DisplayMoney(int deltaMoney)
        {
            _moneyView.SetMoneyText(_playerModel.MoneyAmount.ToString());
        }
    }
}