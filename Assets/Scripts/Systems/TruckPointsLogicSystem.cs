using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.People;
using Model.ShopObjects;
using UnityEngine;

namespace Systems
{
    public class TruckPointsLogicSystem : ISystem
    {
        private const int StaffWorkTimeForHiringByMoney = 60;
        private const int StaffWorkTimeForHiringByAds = 5 * StaffWorkTimeForHiringByMoney;

        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IUpgradeCostProvider _upgradeCostProvider = Instance.Get<IUpgradeCostProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IHireStaffCostProvider _hireStaffCostProvider = Instance.Get<IHireStaffCostProvider>();

        private readonly List<TruckPointModel> _truckPointList = new();

        private readonly Vector2Int[] _staffInitialPointOffsets =
        {
            Vector2Int.right + 2 * Vector2Int.down,
            Vector2Int.right + Vector2Int.up,
        };

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
            _eventBus.Subscribe<TruckPointHireStaffButtonClickedEvent>(OnTruckPointHireStaffButtonClickedEvent);
        }

        private void Unsubscribe()
        {
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
            _updatesProvider.SecondPassed -= OnSecondPassed;
            
            _eventBus.Unsubscribe<TruckArriveAnimationFinishedEvent>(OnTruckArriveAnimationFinished);
            _eventBus.Unsubscribe<UpgradeTruckPointButtonClickedEvent>(OnUpgradeTruckPointButtonClickedEvent);
            _eventBus.Unsubscribe<TruckPointHireStaffButtonClickedEvent>(OnTruckPointHireStaffButtonClickedEvent);
        }

        private void OnTruckArriveAnimationFinished(TruckArriveAnimationFinishedEvent e)
        {
            e.TruckPointModel.ResetProducts();
        }

        private void OnSecondPassed()
        {
            foreach (var truckPointModel in _truckPointList)
            {
                ProcessDeliverLogic(truckPointModel);
                ProcessStaffLogic(truckPointModel);
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

        private void ProcessStaffLogic(TruckPointModel truckPointModel)
        {
            for (var i = 0; i < truckPointModel.StaffCharModels.Count; i++)
            {
                var staffCharModel = truckPointModel.StaffCharModels[i];
                if (staffCharModel != null)
                {
                    if (staffCharModel.WorkSecondsLeft <= 0)
                    {
                        if (staffCharModel.HasProducts == false)
                        {
                            truckPointModel.RemoveStaff(staffCharModel);
                        }
                    }
                    else
                    {
                        staffCharModel.AdvanceWorkingTime();
                    }
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

        private void OnTruckPointHireStaffButtonClickedEvent(TruckPointHireStaffButtonClickedEvent e)
        {
            var truckPointModel = e.TruckPointModel;
            var hireCost = _hireStaffCostProvider.GetTruckPointHireStaffCost(truckPointModel);
            
            if (truckPointModel.CanAddStaff() == false) return;
            
            if (hireCost > 0)
            {
                if (_playerModel.TrySpendMoney(hireCost))
                {
                    HireNewStaffTo(truckPointModel, StaffWorkTimeForHiringByMoney);
                }
            }
            else if (hireCost == HireStaffCostProvider.HireStaffWatchAdsCost)
            {
                //process watch ads and hire
                //HireNewStaffTo(truckPointModel, StaffWorkTimeForHiringByAds);
            }
        }

        private void HireNewStaffTo(TruckPointModel truckPointModel, int workTime)
        {
            var slotIndex = truckPointModel.GetReadyToHireStaffSlotIndex();
            var offset = slotIndex < _staffInitialPointOffsets.Length
                ? _staffInitialPointOffsets[slotIndex]
                : _staffInitialPointOffsets[0];
            
            var staffInitialPosition = truckPointModel.CellCoords + offset;
            
            var staffModel = new TruckPointStaffCharModelBase(
                staffInitialPosition,
                workSecondsLeft: workTime,
                workSecondsLeftSetting: workTime,
                Array.Empty<ProductType>());

            truckPointModel.AddStaffToFreeSlot(staffModel);
        }
    }
}