using Data;
using Events;
using Extensions;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using Tools.AudioManager;

namespace Systems
{
    public class TruckPointsLogicSystem : ISystem
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IUpgradeCostProvider _upgradeCostProvider = Instance.Get<IUpgradeCostProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private ShopModel _shopModel;
        private PlayerModel _playerModel;

        public void Start()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            _shopModel = _playerModelHolder.PlayerModel.ShopModel;
            
            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _updatesProvider.GameplaySecondPassed += OnSecondPassed;
            
            _eventBus.Subscribe<TruckArriveAnimationFinishedEvent>(OnTruckArriveAnimationFinished);
            _eventBus.Subscribe<UpgradeTruckPointButtonClickedEvent>(OnUpgradeTruckPointButtonClickedEvent);
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplaySecondPassed -= OnSecondPassed;
            
            _eventBus.Unsubscribe<TruckArriveAnimationFinishedEvent>(OnTruckArriveAnimationFinished);
            _eventBus.Unsubscribe<UpgradeTruckPointButtonClickedEvent>(OnUpgradeTruckPointButtonClickedEvent);
        }

        private void OnTruckArriveAnimationFinished(TruckArriveAnimationFinishedEvent e)
        {
            e.TruckPointModel.ResetProducts();
        }

        private void OnSecondPassed()
        {
            foreach (var truckPointModel in _shopModel.TruckPoints)
            {
                ProcessDeliverLogic(truckPointModel);
            }
        }

        private void ProcessDeliverLogic(TruckPointModel truckPointModel)
        {
            var isTimeAdvanced = truckPointModel.AdvanceDeliverTime();

            if (isTimeAdvanced && truckPointModel.DeliverTimeSecondsRest <= 0)
            {
                _eventBus.Dispatch(new TruckArrivedEvent(truckPointModel));
                return;
            }

            if (truckPointModel.DeliverTimeSecondsRest <= 0
                && truckPointModel.HasProducts == false)
            {
                truckPointModel.ResetDeliverTime();
            }
        }

        private void OnUpgradeTruckPointButtonClickedEvent(UpgradeTruckPointButtonClickedEvent e)
        {
            var truckPointModel = e.TargetTruckPoint;
            if (truckPointModel.CanUpgrade())
            {
                var upgradeCost = _upgradeCostProvider.GetTruckPointUpgradeCost(truckPointModel);
                if (_playerModel.TrySpendMoney(upgradeCost))
                {
                    _audioPlayer.PlaySound(SoundIdKey.CashSound_2);
                    
                    truckPointModel.Upgrade();
                    
                    _eventBus.Dispatch(new TruckPointUpgradedEvent(truckPointModel));
                }
            }
        }
    }
}