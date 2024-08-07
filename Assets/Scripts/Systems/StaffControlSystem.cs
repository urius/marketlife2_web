using System.Collections.Generic;
using System.Linq;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.People;
using Model.People.States;
using Model.People.States.Staff;
using Model.ShopObjects;
using UnityEngine;
using Utils;
using View.Helpers;

namespace Systems
{
    public class StaffControlSystem : BotCharsControlSystemBase
    {
        private const int StaffWorkTimeForHiringByMoney = 60;
        private const int StaffWorkTimeForHiringByAds = 5 * StaffWorkTimeForHiringByMoney;

        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IHireStaffCostProvider _hireStaffCostProvider = Instance.Get<IHireStaffCostProvider>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();

        private readonly Dictionary<TruckPointStaffCharModel, TruckPointModel> _truckPointByStaffModel = new();
        private readonly Vector2Int[] _staffInitialPointOffsets =
        {
            Vector2Int.right + 2 * Vector2Int.down,
            Vector2Int.right + Vector2Int.up,
        };
        
        private ShopModel _shopModel;
        private BotCharsOwnedCellModel _truckPointStaffOwnedCellModel;
        private PlayerModel _playerModel;

        public override void Start()
        {
            base.Start();

            _playerModel = _playerModelHolder.PlayerModel;
            _shopModel = _playerModel.ShopModel;
            _truckPointStaffOwnedCellModel = _shopModel.TruckPointStaffOwnedCellModel;

            ConsiderExistingStaffCharModels();

            Subscribe();
        }

        public override void Stop()
        {
            Unsubscribe();
            
            base.Stop();
        }

        protected override void BeforeChangeCell(BotCharModelBase charModel, Vector2Int stepCell)
        {
            _truckPointStaffOwnedCellModel.SetOwnedCell(charModel,  stepCell);
        }
        
        private void Subscribe()
        {
            _eventBus.Subscribe<TruckPointHireStaffButtonClickedEvent>(OnTruckPointHireStaffButtonClickedEvent);
            _eventBus.Subscribe<TrucPointStaffStepFinishedEvent>(OnStaffStepFinishedEvent);
            _eventBus.Subscribe<StaffTakeBoxFromTruckAnimationFinishedEvent>(OnStaffTakeBoxFromTruckAnimationFinishedEvent);
            _eventBus.Subscribe<PutProductOnShelfHalfAnimationEvent>(OnPutProductOnShelfHalfAnimationEvent);
            
            _updatesProvider.SecondPassed += OnSecondPassed;
        }

        private void Unsubscribe()
        {            
            _eventBus.Unsubscribe<TruckPointHireStaffButtonClickedEvent>(OnTruckPointHireStaffButtonClickedEvent);
            _eventBus.Unsubscribe<TrucPointStaffStepFinishedEvent>(OnStaffStepFinishedEvent);
            _eventBus.Unsubscribe<StaffTakeBoxFromTruckAnimationFinishedEvent>(OnStaffTakeBoxFromTruckAnimationFinishedEvent);
            _eventBus.Unsubscribe<PutProductOnShelfHalfAnimationEvent>(OnPutProductOnShelfHalfAnimationEvent);
            
            _updatesProvider.SecondPassed -= OnSecondPassed;
        }

        private void OnPutProductOnShelfHalfAnimationEvent(PutProductOnShelfHalfAnimationEvent e)
        {
            foreach (var truckPointModel in _shopModel.TruckPoints)
            {
                foreach (var staffCharModel in truckPointModel.StaffCharModels)
                {
                    if (staffCharModel != null 
                        && staffCharModel.ProductsBox == e.ProductBoxModel)
                    {
                        var state = (TruckPointStaffPutProductsOnShelfState)staffCharModel.State;
                        var triggerPutNextProductResult = TryPutNextProductOnShelf(staffCharModel, state.TargetShelf);

                        if (triggerPutNextProductResult == false)
                        {
                            ProcessNextState(staffCharModel);
                        }

                        return;
                    }
                }
            }
        }

        private void OnStaffTakeBoxFromTruckAnimationFinishedEvent(StaffTakeBoxFromTruckAnimationFinishedEvent e)
        {
            ProcessNextState(e.CharModel);
        }

        private void OnSecondPassed()
        {
            foreach (var truckPointModel in _shopModel.TruckPoints)
            {
                ProcessTruckPointStaff(truckPointModel);
            }
        }

        private void ProcessTruckPointStaff(TruckPointModel truckPointModel)
        {
            foreach (var staffCharModel in truckPointModel.StaffCharModels)
            {
                if (staffCharModel != null)
                {
                    ProcessIdleStaffIfNeeded(staffCharModel);
                    
                    ProcessStaffTimeLogic(truckPointModel, staffCharModel);
                }
            }
        }

        private void ProcessStaffTimeLogic(TruckPointModel truckPointModel, TruckPointStaffCharModel staffCharModel)
        {
            if (staffCharModel.WorkSecondsLeft <= 0)
            {
                if (staffCharModel.HasProducts == false)
                {
                    truckPointModel.RemoveStaff(staffCharModel);
                    ConsiderStaffRemoved(staffCharModel);
                }
            }
            else if (staffCharModel.WorkSecondsLeft > 1 || staffCharModel.HasProducts == false)
            {
                staffCharModel.AdvanceWorkingTime();
            }
        }

        private void ProcessIdleStaffIfNeeded(TruckPointStaffCharModel staffCharModel)
        {
            if (staffCharModel.State == null 
                || staffCharModel.State.StateName == ShopCharStateName.TpStaffIdle)
            {
                ProcessNextState(staffCharModel);
            }
        }

        private void ConsiderExistingStaffCharModels()
        {
            foreach (var truckPointModel in _shopModel.TruckPoints)
            {
                foreach (var staffCharModel in truckPointModel.StaffCharModels)
                {
                    if (staffCharModel == null) continue;

                    ConsiderNewStaff(staffCharModel, truckPointModel);
                }
            }
        }

        private void ConsiderNewStaff(TruckPointStaffCharModel staffCharModel, TruckPointModel truckPointModel)
        {
            _truckPointStaffOwnedCellModel.SetOwnedCell(staffCharModel, staffCharModel.CellCoords);
            _truckPointByStaffModel[staffCharModel] = truckPointModel;
        }

        private void ConsiderStaffRemoved(TruckPointStaffCharModel charModel)
        {
            _truckPointStaffOwnedCellModel.TryRemoveOwnedCell(charModel);
            _truckPointByStaffModel.Remove(charModel);
        }

        private void ProcessNextState(TruckPointStaffCharModel charModel)
        {
            var refTruckPointModel = _truckPointByStaffModel[charModel];

            if (charModel.HasProducts == false)
            {
                if (refTruckPointModel.IsDelivered)
                {
                    if (TruckPointHelper.IsOnInteractionPoint(refTruckPointModel.CellCoords, charModel.CellCoords))
                    {
                        SetTakeProductFromTruckPointState(charModel, refTruckPointModel);
                    }
                    else
                    {
                        SetMoveToTruckPointState(charModel, refTruckPointModel);
                    }
                }
                else
                {
                    if (charModel.State?.StateName 
                        is ShopCharStateName.TpStaffMovingToTruckPointWaitingCell 
                        or ShopCharStateName.TpStaffIdle)
                    {
                        SetIdleState(charModel);
                    }
                    else
                    {
                        SetMoveToTruckPointWaitingCellState(charModel, refTruckPointModel);
                    }
                }
            }
            else if (charModel.HasProducts)
            {
                if (charModel.State?.StateName == ShopCharStateName.TpStaffMovingToShelf)
                {
                    var movingToShelfState = (TruckPointStaffMovingToShelfState)charModel.State;
                    if (TrySetPutProductOnShelfState(charModel, movingToShelfState.TargetShelf))
                    {
                        return;
                    }
                }

                SetMoveToShelfStateOrIdle(charModel);
            }
        }

        private bool TrySetPutProductOnShelfState(TruckPointStaffCharModel charModel, ShelfModel targetShelf)
        {
            if (TryPutNextProductOnShelf(charModel, targetShelf))
            {
                charModel.SetPutProductsToShelfState(targetShelf);
                return true;
            }

            return false;
        }

        private bool TryPutNextProductOnShelf(TruckPointStaffCharModel charModel, ShelfModel targetShelf)
        {
            var emptyShelfSlotIndex = targetShelf.GetEmptySlotIndex();
            if (emptyShelfSlotIndex >= 0)
            {
                var staffProductsBoxIndex = charModel.GetNextNotEmptySlotIndex();
                if (staffProductsBoxIndex >= 0)
                {
                    var productToPut = charModel.ProductsInBox[staffProductsBoxIndex];
                    charModel.RemoveProductFromSlot(staffProductsBoxIndex);
                    targetShelf.AddProductToSlot(emptyShelfSlotIndex, productToPut);

                    _eventBus.Dispatch(new AnimatePutProductOnShelfEvent(targetShelf, charModel.ProductsBox,
                        productToPut, staffProductsBoxIndex, emptyShelfSlotIndex));

                    return true;
                }
            }

            return false;
        }

        private void SetMoveToShelfStateOrIdle(TruckPointStaffCharModel charModel)
        {
            var setStateSuccessFlag = TrySetMoveToShelfState(charModel);
            if (!setStateSuccessFlag && charModel.State != null)
            {
                SetIdleState(charModel);
            }
        }

        private static void SetIdleState(TruckPointStaffCharModel charModel)
        {
            charModel.SetIdleState();
        }

        private bool TrySetMoveToShelfState(TruckPointStaffCharModel charModel)
        {
            var allShelfs = _shopModel.Shelfs;
            var targetShelf = allShelfs.FirstOrDefault(s => s.HasEmptySlots());
            if (targetShelf != null)
            {
                var targetCell = GetShelfClosestNearPoint(targetShelf, charModel.CellCoords);
                
                charModel.SetMovingToShelfState(targetCell, targetShelf);

                MakeNextStep(charModel, targetCell);

                return true;
            }

            return false;
        }

        private Vector2Int GetShelfClosestNearPoint(ShelfModel targetShelf, Vector2Int cellCoords)
        {
            var ownedCells = _ownedCellsDataHolder.GetShopObjectOwnedCells(targetShelf);
            var closestCell = Vector2Int.zero;
            var closestDistance = float.MaxValue;

            foreach (var ownedCell in ownedCells)
            {
                foreach (var cellOffset in Constants.NearCells8)
                {
                    var nearCell = ownedCell + cellOffset;
                    
                    var distance = Vector2Int.Distance(nearCell, cellCoords);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCell = nearCell;
                    }
                }
            }
            
            return closestCell;
        }

        private void SetTakeProductFromTruckPointState(TruckPointStaffCharModel charModel, TruckPointModel truckPointModel)
        {
            var productBoxIndexToTake = truckPointModel.GetLastNotEmptyProductBoxIndex();
            var productTypeToTake = truckPointModel.GetProductTypeAtBoxIndex(productBoxIndexToTake);
                    
            truckPointModel.RemoveBox(productBoxIndexToTake);
            charModel.FillBoxWithProduct(productTypeToTake);
            
            charModel.SetTakeProductFromTruckPointState(truckPointModel, productBoxIndexToTake);
        }

        private void OnStaffStepFinishedEvent(TrucPointStaffStepFinishedEvent e)
        {
            var charModel = e.CharModel;
            var movingState = (BotCharMovingStateBase)charModel.State;
            
            charModel.IsStepInProgress = false;

            if (movingState.TargetCell == charModel.CellCoords)
            {
                ProcessNextState(charModel);
            }
            else
            {
                if (movingState.StateName == ShopCharStateName.TpStaffMovingToShelf)
                {
                    var moveToShelfState = (TruckPointStaffMovingToShelfState)movingState;
                    if (IsNearToShelf(charModel.CellCoords, moveToShelfState.TargetShelf))
                    {
                        ProcessNextState(charModel);
                        return;
                    }
                }

                MakeNextStep(charModel, movingState.TargetCell);
            }
        }

        private void SetMoveToTruckPointState(TruckPointStaffCharModel charModel, TruckPointModel refTruckPointModel)
        {
            var closestInteractionCell =
                TruckPointHelper.GetClosestInteractionCell(refTruckPointModel.CellCoords, charModel.CellCoords);
            
            charModel.SetMoveToTruckPointState(refTruckPointModel, closestInteractionCell);

            MakeNextStep(charModel, closestInteractionCell);
        }
        
        private void SetMoveToTruckPointWaitingCellState(TruckPointStaffCharModel charModel, TruckPointModel refTruckPointModel)
        {
            var index = GetStaffIndexInTruckPointModel(charModel);
            if (index >= 0)
            {
                var targetPoint = TruckPointHelper.GetTruckPointWaitingPoint(refTruckPointModel.CellCoords, index);
            
                charModel.SetMoveToTruckPointWaitingCellState(refTruckPointModel, targetPoint);

                MakeNextStep(charModel, targetPoint);
            }
        }

        private int GetStaffIndexInTruckPointModel(TruckPointStaffCharModel charModel)
        {
            foreach (var truckPoint in _shopModel.TruckPoints)
            {
                for (var i = 0; i < truckPoint.StaffCharModels.Count; i++)
                {
                    if (truckPoint.StaffCharModels[i] == charModel) return i;
                }
            }

            return -1;
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

            var staffModel = new TruckPointStaffCharModel(staffInitialPosition, workTime);

            ConsiderNewStaff(staffModel, truckPointModel);
            
            truckPointModel.AddStaffToFreeSlot(staffModel);
        }

        private bool IsNearToShelf(Vector2Int cellCoords, ShelfModel shelfModel)
        {
            var ownedCells = _ownedCellsDataHolder.GetShopObjectOwnedCells(shelfModel);
            foreach (var ownedCell in ownedCells)
            {
                if (_gridCalculator.AreCellsNear(cellCoords, ownedCell))
                {
                    return true;
                }
            }

            return false;
        }
    }
}