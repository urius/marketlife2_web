using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using Model;
using Model.People.States.Customer;
using Model.ShopObjects;
using UnityEngine;
using Utils;

namespace View.Game.People
{
    public class PlayerCharCompassMediator : MediatorBase
    {
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        
        private readonly ManView _playerCharView;
        private readonly LinkedList<CompassDataBase> _compassDataList = new();
        
        private ShopModel _shopModel;
        private PlayerCharModel _playerCharModel;
        private bool _addCompassPhaseFlag;

        public PlayerCharCompassMediator(ManView playerCharView)
        {
            _playerCharView = playerCharView;
        }

        protected override void MediateInternal()
        {
            _playerCharModel = _playerModelHolder.PlayerCharModel;
            _shopModel = _playerModelHolder.PlayerModel.ShopModel;
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();

            foreach (var compassData in _compassDataList)
            {
                if (compassData.CompassView != null)
                {
                    Destroy(compassData.CompassView);
                }
            }
            _compassDataList.Clear();
        }

        private void Subscribe()
        {
            _updatesProvider.SecondPassed += OnSecondPassed;
            _updatesProvider.QuarterSecondPassed += OnQuarterSecondPassed;
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
        }

        private void Unsubscribe()
        {
            _updatesProvider.SecondPassed -= OnSecondPassed;
            _updatesProvider.QuarterSecondPassed -= OnQuarterSecondPassed;
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
        }

        private void OnQuarterSecondPassed()
        {
            RemoveCompassesIfNeeded();
        }

        private void OnSecondPassed()
        {
            AddNewCompassesIfNeeded();
        }

        private bool AddNewCompassesIfNeeded()
        {
            var result = false;

            if (CheckCompassExists(CompassType.CashDeskCompass) == false)
            {
                var waitingCustomer = _shopModel.CustomersModel.GetWaitingCustomer();

                var targetCashDesk = (waitingCustomer?.State as CustomerMovingToCashDeskState)?.TargetCashDesk;
                if (targetCashDesk != null
                    && targetCashDesk.HasCashMan == false
                    && _playerCharModel.NearCashDesk != targetCashDesk)
                {
                    AddCashDeskCompass(targetCashDesk);
                    result = true;
                }
            }

            if (_playerCharModel.HasProducts == false
                && CheckCompassExists(CompassType.TakeProductFromTruckPointCompass) == false)
            {
                foreach (var truckPointModel in _shopModel.TruckPoints)
                {
                    if (truckPointModel.IsDelivered
                        && truckPointModel.HasStaff == false
                        && _playerCharModel.NearTruckPoint != truckPointModel)
                    {
                        AddTruckPointCompass(truckPointModel);
                        result = true;
                    }
                }
            }
            else if (_playerCharModel.HasProducts
                     && CheckCompassExists(CompassType.PlaceProductOnShelfCompass) == false)
            {
                foreach (var shelfModel in _shopModel.Shelfs)
                {
                    if (shelfModel.HasEmptySlots())
                    {
                        AddShelfCompass(shelfModel);
                        result = true;
                    }
                }
            }

            return result;
        }

        private bool RemoveCompassesIfNeeded()
        {
            foreach (var compassData in _compassDataList)
            {
                if (TryRemoveCompass(compassData))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryRemoveCompass(CompassDataBase compassData)
        {
            if (compassData.CompassType == CompassType.CashDeskCompass)
            {
                if (((CashDeskCompassData)compassData).TargetCashDeskModel == _playerCharModel.NearCashDesk)
                {
                    RemoveCompass(compassData);
                    return true;
                }
            }
            else if (compassData.CompassType == CompassType.PlaceProductOnShelfCompass)
            {
                if (_playerCharModel.HasProducts == false
                    || ((ShelfCompassData)compassData).TargetCashDeskModel.HasEmptySlots() == false)
                {
                    RemoveCompass(compassData);
                    return true;
                }
            }
            else if (compassData.CompassType == CompassType.TakeProductFromTruckPointCompass)
            {
                if (_playerCharModel.HasProducts
                    || ((TruckPointCompassData)compassData).TargetTruckPointModel == _playerCharModel.NearTruckPoint)
                {
                    RemoveCompass(compassData);
                    return true;
                }
            }

            return false;
        }

        private void RemoveCompass(CompassDataBase compassData)
        {
            _compassDataList.Remove(compassData);
            Destroy(compassData.CompassView);
        }

        private bool CheckCompassExists(CompassType compassType)
        {
            foreach (var compassData in _compassDataList)
            {
                if (compassData.CompassType == compassType)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnGameplayFixedUpdate()
        {
            var playerCharPosition = _playerCharView.transform.position;
            foreach (var compassData in _compassDataList)
            {
                compassData.CompassView.SetPosition(playerCharPosition);
                compassData.CompassView.SetLookToPosition(compassData.TargetCoords);

                var distanceSqr = (compassData.TargetCoords - playerCharPosition).sqrMagnitude;
                compassData.CompassView.SetArrowDistancePercent(distanceSqr);
                compassData.CompassView.SetArrowAlphaPercent(distanceSqr * 0.5f);
            }
        }

        private void AddCashDeskCompass(CashDeskModel targetCashDeskModel)
        {
            var compassView = InstantiatePrefab<PlayerCompassView>(PrefabKey.ManCompass);
            
            var targetWorldPosition = _gridCalculator.GetCellCenterWorld(targetCashDeskModel.CellCoords);
            var compassData = new CashDeskCompassData(compassView, targetWorldPosition, targetCashDeskModel);

            _compassDataList.AddLast(compassData);
        }

        private void AddShelfCompass(ShelfModel targetShelfModel)
        {
            var compassView = InstantiatePrefab<PlayerCompassView>(PrefabKey.ManCompass);
            
            var targetWorldPosition = _gridCalculator.GetCellCenterWorld(targetShelfModel.CellCoords);
            var compassData = new ShelfCompassData(compassView, targetWorldPosition, targetShelfModel);

            _compassDataList.AddLast(compassData);
        }

        private void AddTruckPointCompass(TruckPointModel truckPointModel)
        {
            var compassView = InstantiatePrefab<PlayerCompassView>(PrefabKey.ManCompass);
            
            var targetWorldPosition = _gridCalculator.GetCellCenterWorld(truckPointModel.CellCoords);
            var compassData = new TruckPointCompassData(compassView, targetWorldPosition, truckPointModel);

            _compassDataList.AddLast(compassData);
        }

        private class CashDeskCompassData : CompassDataBase
        {
            public readonly CashDeskModel TargetCashDeskModel;

            public CashDeskCompassData(
                PlayerCompassView compassView,
                Vector3 targetCoords,
                CashDeskModel targetCashDeskModel) 
                : base(compassView, targetCoords)
            {
                TargetCashDeskModel = targetCashDeskModel;
            }

            public override CompassType CompassType => CompassType.CashDeskCompass;
        }
        
        private class ShelfCompassData : CompassDataBase
        {
            public readonly ShelfModel TargetCashDeskModel;

            public ShelfCompassData(
                PlayerCompassView compassView,
                Vector3 targetCoords,
                ShelfModel targetShelfModel) 
                : base(compassView, targetCoords)
            {
                TargetCashDeskModel = targetShelfModel;
            }

            public override CompassType CompassType => CompassType.PlaceProductOnShelfCompass;
        }
        
        private class TruckPointCompassData : CompassDataBase
        {
            public readonly TruckPointModel TargetTruckPointModel;

            public TruckPointCompassData(
                PlayerCompassView compassView,
                Vector3 targetCoords,
                TruckPointModel targetShelfModel) 
                : base(compassView, targetCoords)
            {
                TargetTruckPointModel = targetShelfModel;
            }

            public override CompassType CompassType => CompassType.TakeProductFromTruckPointCompass;
        }
        
        private abstract class CompassDataBase
        {
            public readonly Vector3 TargetCoords;
            public readonly PlayerCompassView CompassView;

            public CompassDataBase(PlayerCompassView compassView, Vector3 targetCoords)
            {
                CompassView = compassView;
                TargetCoords = targetCoords;
            }
            
            public abstract CompassType CompassType { get; }
        }
        
        private enum CompassType
        {
            Undefined,
            CashDeskCompass,
            TakeProductFromTruckPointCompass,
            PlaceProductOnShelfCompass,
        }
    }
}