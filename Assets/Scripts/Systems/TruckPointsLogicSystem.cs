using System.Collections.Generic;
using System.Linq;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.ShopObjects;

namespace Systems
{
    public class TruckPointsLogicSystem : ISystem
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IUpgradeCostProvider _upgradeCostProvider = Instance.Get<IUpgradeCostProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private readonly List<TruckPointModel> _truckPointList = new();

        private ShopModel _shopModel;
        private PlayerModel _playerModel;

        public void Start()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            _shopModel = _playerModelHolder.PlayerModel.ShopModel;
                
            PopulateTruckPointModels();

            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _shopModel.ShopObjectAdded += OnShopObjectAdded;
            _updatesProvider.SecondPassed += OnSecondPassed;
            _eventBus.Subscribe<TruckArriveAnimationFinishedEvent>(OnTruckArriveAnimationFinished);
            
            _eventBus.Subscribe<UpgradeTruckPointButtonClickedEvent>(OnUpgradeTruckPointButtonClickedEvent);
        }

        private void Unsubscribe()
        {
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
            _updatesProvider.SecondPassed -= OnSecondPassed;
            _eventBus.Unsubscribe<TruckArriveAnimationFinishedEvent>(OnTruckArriveAnimationFinished);
            
            _eventBus.Unsubscribe<UpgradeTruckPointButtonClickedEvent>(OnUpgradeTruckPointButtonClickedEvent);
        }

        private void OnTruckArriveAnimationFinished(TruckArriveAnimationFinishedEvent e)
        {
            e.TruckPointModel.ResetProducts();
        }

        private void OnSecondPassed()
        {
            foreach (var truckPointModel in _truckPointList)
            {
                var isAdvanced = truckPointModel.AdvanceDeliverTime();
                
                if (isAdvanced && truckPointModel.DeliverTimeSecondsRest <= 0)
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
        }

        private void OnShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            if (shopObjectModel.ShopObjectType == ShopObjectType.TruckPoint)
            {
                _truckPointList.Add((TruckPointModel)shopObjectModel);
            }
        }

        private void PopulateTruckPointModels()
        {
            var truckPoints = _shopModel.ShopObjects.Values
                .Where(o => o.ShopObjectType == ShopObjectType.TruckPoint)
                .Cast<TruckPointModel>()
                .ToArray();

            _truckPointList.Capacity = truckPoints.Length * 2;
            
            _truckPointList.AddRange(truckPoints);
        }

        private void OnUpgradeTruckPointButtonClickedEvent(UpgradeTruckPointButtonClickedEvent e)
        {
            var truckPointModel = e.TargetTruckPoint;
            if (truckPointModel.CanUpgrade())
            {
                var upgradeCost = _upgradeCostProvider.GetTruckPointUpgradeCost(truckPointModel);
                if (_playerModel.TrySpendMoney(upgradeCost))
                {
                    truckPointModel.Upgrade();
                }
            }
        }
    }
}