using System.Collections.Generic;
using System.Linq;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.Customers;
using Model.Customers.States;
using Model.ShopObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems
{
    public class CustomersControlSystem : ISystem
    {
        private const int SpawnY = -5;
        private const int DespawnY = -6;
        
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private readonly List<ShelfModel> _allShelfs = new ();
        private readonly List<CashDeskModel> _allCashDesks = new ();
        private readonly Dictionary<CashDeskModel, List<CustomerCharModel>> _cashDeskCustomerQueues = new ();

        private ShopModel _shopModel;
        private PlayerCharModel _playerCharModel;
        private CustomersModel _customersModel;
        private int _processNextIdleCustomerIndex = -1;
        
        public void Start()
        {
            _shopModel = _shopModelHolder.ShopModel;
            _customersModel = _shopModel.CustomersModel;
            _playerCharModel = _playerModelHolder.PlayerCharModel;

            PopulateShelfs();
            PopulateCashDesks();
            UpdateMaxCustomersAmount();

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
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
            _eventBus.Subscribe<CustomerInitializedEvent>(OnCustomerInitializedEvent);
            _eventBus.Subscribe<CustomerStepFinishedEvent>(OnCustomerStepFinishedEvent);
            _eventBus.Subscribe<CustomerTakeProductAnimationFinishedEvent>(OnCustomerTakeProductAnimationFinishedEvent);
            _eventBus.Subscribe<CustomerFlyProductFromBasketAnimationFinishedEvent>(OnCustomerFlyProductFromBasketAnimationFinishedEvent);
        }

        private void Unsubscribe()
        {
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
            _updatesProvider.SecondPassed -= OnSecondPassed;
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            _eventBus.Unsubscribe<CustomerInitializedEvent>(OnCustomerInitializedEvent);
            _eventBus.Unsubscribe<CustomerStepFinishedEvent>(OnCustomerStepFinishedEvent);
            _eventBus.Unsubscribe<CustomerTakeProductAnimationFinishedEvent>(OnCustomerTakeProductAnimationFinishedEvent);
            _eventBus.Unsubscribe<CustomerFlyProductFromBasketAnimationFinishedEvent>(OnCustomerFlyProductFromBasketAnimationFinishedEvent);
        }

        private void OnCustomerInitializedEvent(CustomerInitializedEvent e)
        {
            SetMoveToEnterState(e.CustomerModel);
        }

        private void OnCustomerStepFinishedEvent(CustomerStepFinishedEvent e)
        {
            var customerModel = e.CustomerModel;
            var movingState = (CustomerMovingStateBase)customerModel.State;
            
            customerModel.IsStepInProgress = false;

            if (movingState.TargetCell == customerModel.CellPosition)
            {
                ProcessNextState(customerModel);
            }
            else
            {
                if (movingState.StateName == CustomerGlobalStateName.MovingToCashDesk)
                {
                    var cashDesk = ((CustomerMovingToCashDeskState)movingState).TargetCashDesk;
                    var queueIndex = GetCustomerCashDeskQueueIndex(cashDesk, customerModel);
                    var queueTargetCell = GetCashDeskQueueCell(cashDesk, queueIndex);
                    
                    if (customerModel.CellPosition != queueTargetCell)
                    {
                        MakeNextStep(customerModel, queueTargetCell);
                    }
                }
                else
                {
                    MakeNextStep(customerModel, movingState.TargetCell);
                }
            }
        }
        

        private bool IsOnOwnQueueCell(CashDeskModel cashDeskModel, CustomerCharModel customer, int currentQueueIndex)
        {
            var queueTargetCell = GetCashDeskQueueCell(cashDeskModel, currentQueueIndex);
            
            return customer.CellPosition == queueTargetCell;
        }
        
        private bool TryMoveToQueueCell(CashDeskModel cashDeskModel, CustomerCharModel customerModel, int currentQueueIndex)
        {
            var queueTargetCell = GetCashDeskQueueCell(cashDeskModel, currentQueueIndex);
            var isCellFree = IsCellFree(queueTargetCell);

            if (isCellFree)
            {
                MakeNextStep(customerModel, queueTargetCell);
            }

            return isCellFree;
        }

        private bool TryAdvanceInQueue(CashDeskModel cashDeskModel, CustomerCharModel customerModel, int currentQueueIndex)
        {
            if (currentQueueIndex <= 0) return false;
            
            var nextQueueIndex = currentQueueIndex - 1;
            var nextQueueTargetCell = GetCashDeskQueueCell(cashDeskModel, nextQueueIndex);
            var isCellFree = IsCellFree(nextQueueTargetCell);

            if (isCellFree)
            {
                SwapCustomersInQueue(cashDeskModel, currentQueueIndex, nextQueueIndex);
            }

            return isCellFree;
        }

        private void SwapCustomersInQueue(CashDeskModel cashDeskModel, int prevQueueIndex, int queueIndex)
        {
            var queue = _cashDeskCustomerQueues[cashDeskModel];
            (queue[prevQueueIndex], queue[queueIndex]) = (queue[queueIndex], queue[prevQueueIndex]);
        }

        private bool IsCellFree(Vector2Int cellPosition)
        {
            return _customersModel.HaveCustomerOnCell(cellPosition) == false;
        }

        private void MakeNextStep(CustomerCharModel customerCharModel, Vector2Int targetPoint)
        {
            var stepCell = Vector2Int.zero;
            var minDistanceToTarget = -1f;
            
            foreach (var nearCellOffset in Constants.NearCells4)
            {
                var nearCell = customerCharModel.CellPosition + nearCellOffset;
                
                if (nearCell == customerCharModel.PreviousCellPosition
                    || CanMakeStepTo(nearCell) == false)
                {
                    continue;
                }

                var distanceToTarget = (targetPoint - nearCell).magnitude;
                
                if (minDistanceToTarget < 0 || distanceToTarget < minDistanceToTarget)
                {
                    minDistanceToTarget = distanceToTarget;
                    stepCell = nearCell;
                }
            }

            if (minDistanceToTarget < 0 && CanMakeStepTo(customerCharModel.PreviousCellPosition))
            {
                stepCell = customerCharModel.PreviousCellPosition;
                minDistanceToTarget = (targetPoint - stepCell).magnitude;
            }

            if (minDistanceToTarget >= 0)
            {
                SetCustomerCellPosition(customerCharModel, stepCell);
            }
            else
            {
                Debug.LogWarning("No cell to move for customer");
            }
        }

        private void SetCustomerCellPosition(CustomerCharModel customerCharModel, Vector2Int cell)
        {
            _customersModel.SetOwnedCell(customerCharModel, cell);
            
            customerCharModel.SetCellPosition(cell);
            customerCharModel.IsStepInProgress = true;
        }

        private void ProcessNextState(CustomerCharModel model)
        {
            switch (model.State.StateName)
            {
                case CustomerGlobalStateName.MovingToEnter:
                    SetMoveToRandomShelfState(model);
                    break;
                case CustomerGlobalStateName.MovingToShelf:
                    var targetProduct = ((CustomerMovingToShelfState)model.State).TargetProduct;
                    if (targetProduct != ProductType.None)
                    {
                        var setStateSuccess = TrySetTakeProductState(model);
                        if (setStateSuccess == false)
                        {
                            setStateSuccess = TrySetMoveToShelfWithExactProductState(model, targetProduct);
                            if (setStateSuccess == false)
                            {
                                SetMoveToExitState(model);
                            }
                        }
                    }
                    else
                    {
                        SetMoveToExitState(model);
                    }
                    break;
                case CustomerGlobalStateName.TakingProduct:
                    SetMoveToCashDeskState(model);
                    break;
                case CustomerGlobalStateName.MovingToCashDesk:
                    TrySetPayingState(((CustomerMovingToCashDeskState)model.State).TargetCashDesk, model);
                    break;
                case CustomerGlobalStateName.Paying:
                    var targetCashDesk = ((CustomerPayingState)model.State).TargetCashDesk;
                    AddMoneyToCashDesk(targetCashDesk, model);
                    SetMoveToExitState(model);
                    RemoveFromCashDeskQueue(targetCashDesk);
                    break;
                case CustomerGlobalStateName.MovingToExit:
                    SetMoveOutOfShopState(model);
                    break;
                case CustomerGlobalStateName.MovingToDespawn:
                    _customersModel.RemoveCustomer(model);
                    break;
            }
        }

        private void AddMoneyToCashDesk(CashDeskModel targetCashDesk, CustomerCharModel customerCharModel)
        {
            targetCashDesk.AddMoney(customerCharModel.ProductsCount);
        }

        private void RemoveFromCashDeskQueue(CashDeskModel cashDeskModel)
        {
            var queue = _cashDeskCustomerQueues[cashDeskModel];
            queue.RemoveAt(0);

            foreach (var customer in queue)
            {
                ProcessIdleCustomer(customer);
            }
        }

        private void OnCustomerTakeProductAnimationFinishedEvent(CustomerTakeProductAnimationFinishedEvent e)
        {
            ProcessNextState(e.TargetModel);
        }

        private void OnCustomerFlyProductFromBasketAnimationFinishedEvent(CustomerFlyProductFromBasketAnimationFinishedEvent e)
        {
            ProcessNextState(e.TargetModel);
        }

        private void SetMoveToEnterState(CustomerCharModel model)
        {
            var closestDoor = GetClosestDoor(model.CellPosition);
            var targetPoint = new Vector2Int(closestDoor.Left, 0);

            model.SetMovingToEnterState(targetPoint);

            MakeNextStep(model, targetPoint);
        }

        private void SetMoveOutOfShopState(CustomerCharModel model)
        {
            var despawnPoint = new Vector2Int(Random.Range(0, _shopModel.Size.x), DespawnY);
            
            model.SetMovingToDespawnState(despawnPoint);

            MakeNextStep(model, despawnPoint);
        }

        private void SetMoveToExitState(CustomerCharModel model)
        {
            var closestDoor = GetClosestDoor(model.CellPosition);
            var targetPoint = new Vector2Int(closestDoor.Right, -2);

            model.SetMovingToExitState(targetPoint);

            MakeNextStep(model, targetPoint);
        }

        private void SetMoveToRandomShelfState(CustomerCharModel model)
        {
            var targetShelf = _allShelfs.FirstOrDefault(s => s.HasProducts())
                              ?? _allShelfs[Random.Range(0, _allShelfs.Count)];
            var targetProduct = targetShelf.GetRandomNotEmptyProductOrDefault();

            SetMoveToShelfState(model, targetProduct, targetShelf);
        }

        private bool TrySetMoveToShelfWithExactProductState(CustomerCharModel model, ProductType targetProduct)
        {
            var targetShelf = _allShelfs.FirstOrDefault(s => s.HasProduct(targetProduct));

            if (targetShelf != null)
            {
                SetMoveToShelfState(model, targetProduct, targetShelf);

                return true;
            }

            return false;
        }

        private void SetMoveToShelfState(CustomerCharModel customerModel, ProductType targetProduct, ShelfModel targetShelf)
        {
            var targetCell = GetShelfBuyPoint(targetShelf);

            customerModel.SetMovingToShelfState(targetCell, targetShelf, targetProduct);

            MakeNextStep(customerModel, targetCell);
        }

        private bool TrySetTakeProductState(CustomerCharModel model)
        {
            var currentMoveToShelfState = (CustomerMovingToShelfState)model.State;
            var targetShelfModel = currentMoveToShelfState.TargetShelf;
            var targetProduct = currentMoveToShelfState.TargetProduct;

            if (targetProduct == ProductType.None) return false;
            
            var slotIndex = targetShelfModel.GetProductSlotIndex(targetProduct);

            if (slotIndex < 0) return false;

            targetShelfModel.RemoveProductFromSlotIndex(slotIndex);
            var addedProductSlotIndex = model.AddProduct(targetProduct);
                
            model.SetTakingProductState(targetShelfModel, slotIndex, targetProduct, addedProductSlotIndex);
            
            return true;
        }

        private void SetMoveToCashDeskState(CustomerCharModel customerCharModel)
        {
            var lessBusyCashDesk = GetLessBusyCashDesk();
            var cashDeskPayPoint = GetCashDeskPayPoint(lessBusyCashDesk);
            AddCustomerToCashDeskQueue(lessBusyCashDesk, customerCharModel);
            
            customerCharModel.SetMovingToCashDeskState(lessBusyCashDesk, cashDeskPayPoint);
            
            MakeNextStepToCashDeskQueuePoint(customerCharModel, lessBusyCashDesk);
        }

        private void MakeNextStepToCashDeskQueuePoint(CustomerCharModel customerModel, CashDeskModel cashDesk)
        {
            var queueIndex = GetCustomerCashDeskQueueIndex(cashDesk, customerModel);
            var targetCell = GetCashDeskQueueCell(cashDesk, queueIndex);

            MakeNextStep(customerModel, targetCell);
        }

        private Vector2Int GetCashDeskQueueCell(CashDeskModel cashDeskModel, int queueIndex)
        {
            return GetCashDeskPayPoint(cashDeskModel) + new Vector2Int(0, queueIndex);
        }

        private static Vector2Int GetCashDeskPayPoint(CashDeskModel lessBusyCashDesk)
        {
            return lessBusyCashDesk.CellCoords + new Vector2Int(1, -1);
        }

        private int GetCustomerCashDeskQueueIndex(CashDeskModel cashDeskModel, CustomerCharModel customerCharModel)
        {
            return _cashDeskCustomerQueues[cashDeskModel].IndexOf(customerCharModel);
        }

        private static Vector2Int GetShelfBuyPoint(ShelfModel targetShelf)
        {
            return targetShelf.CellCoords + new Vector2Int(0, 1);
        }

        private bool CanMakeStepTo(Vector2Int cell)
        {
            return IsWalkable(cell) && !_customersModel.HaveCustomerOnCell(cell);
        }

        private bool IsWalkable(Vector2Int cell)
        {
            if (cell.x < 0 || cell.x >= _shopModel.Size.x) return false;
            if (cell.y < Constants.YTopWalkableCoordForCustomers || cell.y >= _shopModel.Size.y) return false;
            if (cell.y == -1) return _shopModel.HaveDoorOn(cell.x);
            
            return _ownedCellsDataHolder.IsWalkableForCustomerChar(cell);
        }

        private void UpdateMaxCustomersAmount()
        {
            var amount = 1;
            
            foreach (var kvp in _shopModel.ShopObjects)
            {
                var shopObjectType = kvp.Value.ShopObjectType;
                if (shopObjectType.IsShelf()
                    || shopObjectType == ShopObjectType.CashDesk)
                {
                    amount++;
                }
            }

            _customersModel.MaxCustomersAmount = amount;
        }

        private void OnSecondPassed()
        {
            ProcessSpawnLogic();
        }

        private void OnGameplayFixedUpdate()
        {
            ProcessNextIdleCustomer();
        }

        private void ProcessNextIdleCustomer()
        {
            if (_customersModel.CustomersAmount <= 0) return;

            _processNextIdleCustomerIndex++;

            if (_processNextIdleCustomerIndex >= _customersModel.CustomersAmount)
            {
                _processNextIdleCustomerIndex = 0;
            }

            ProcessIdleCustomer(_customersModel.Customers[_processNextIdleCustomerIndex]);
        }

        private void ProcessIdleCustomer(CustomerCharModel customer)
        {
            if (customer.IsStepInProgress == false
                && customer.State is CustomerMovingStateBase movingStateBase)
            {
                if (movingStateBase.StateName == CustomerGlobalStateName.MovingToCashDesk)
                {
                    var moveToCashDeskState = (CustomerMovingToCashDeskState)movingStateBase;
                    var cashDesk = moveToCashDeskState.TargetCashDesk;
                    var currentQueueIndex = GetCustomerCashDeskQueueIndex(cashDesk, customer);
                    
                    if (currentQueueIndex == 0 && customer.CellPosition == GetCashDeskPayPoint(cashDesk))
                    {
                        TrySetPayingState(cashDesk, customer);
                    }
                    else
                    {
                        if (IsOnOwnQueueCell(cashDesk, customer, currentQueueIndex))
                        {
                            TryAdvanceInQueue(cashDesk, customer, currentQueueIndex);
                        }

                        TryMoveToQueueCell(cashDesk, customer, currentQueueIndex);
                    }
                }
                else
                {
                    MakeNextStep(customer, movingStateBase.TargetCell);
                }
            }
        }

        private bool TrySetPayingState(CashDeskModel cashDesk, CustomerCharModel customer)
        {
            if (cashDesk.HasCashMan || IsPlayerNearCashDesk(cashDesk))
            {
                var movingToCashDeskState = (CustomerMovingToCashDeskState)customer.State;
                customer.SetPayingState(movingToCashDeskState.TargetCashDesk);

                return true;
            }

            return false;
        }

        private bool IsPlayerNearCashDesk(CashDeskModel cashDesk)
        {
            return _playerCharModel.NearCashDesk == cashDesk;
        }

        private void ProcessSpawnLogic()
        {
            _customersModel.AdvanceSpawnCooldown();

            if (_customersModel.SpawnCooldownSecondsLeft <= 0
                && _customersModel.CustomersAmount < _customersModel.MaxCustomersAmount
                && _allShelfs.Count > 0
                && (_allShelfs.Count > 10 || HaveShelfsWithProducts()))
            {
                var spawnPoint = new Vector2Int(Random.Range(0, _shopModel.Size.x), SpawnY);

                if (_customersModel.HaveCustomerOnCell(spawnPoint) == false)
                {
                    _customersModel.ResetSpawnCooldown();

                    var customer = CreateCustomer(spawnPoint);
                    _customersModel.AddCustomer(customer);
                }
            }
        }

        private bool HaveShelfsWithProducts()
        {
            foreach (var shelf in _allShelfs)
            {
                if (shelf.HasProducts())
                {
                    return true;
                }
            }

            return false;
        }

        private CustomerCharModel CreateCustomer(Vector2Int spawnPoint)
        {
            return new CustomerCharModel(spawnPoint);;
        }

        private (int Left, int Right) GetClosestDoor(Vector2Int targetPoint)
        {
            var minDistance = -1f;
            var closestDoorCoords = (Left: -1, Right: -1);

            foreach (var doorCoords in _shopModel.Doors)
            {
                var distance = (new Vector2Int(doorCoords.Left, -1) - targetPoint).magnitude;
                if (closestDoorCoords.Left < 0 || distance < minDistance)
                {
                    closestDoorCoords = doorCoords;
                    minDistance = distance;
                }
            }

            return closestDoorCoords;
        }

        private CashDeskModel GetLessBusyCashDesk()
        {
            CashDeskModel lessBusyCashDesk = null;
            var minBusyValue = int.MaxValue;

            foreach (var cashDeskModel in _allCashDesks)
            {
                var amountOfQueuedCustomers = GetAmountOfQueuedCustomers(cashDeskModel);

                if (amountOfQueuedCustomers < minBusyValue)
                {
                    minBusyValue = amountOfQueuedCustomers;
                    lessBusyCashDesk = cashDeskModel;
                }
            }

            return lessBusyCashDesk;
        }

        private int GetAmountOfQueuedCustomers(CashDeskModel cashDeskModel)
        {
            return _cashDeskCustomerQueues.TryGetValue(cashDeskModel, out var value) ? value.Count : 0;
        }

        private void AddCustomerToCashDeskQueue(CashDeskModel cashDeskModel, CustomerCharModel customerCharModel)
        {
            if (_cashDeskCustomerQueues.ContainsKey(cashDeskModel) == false)
            {
                _cashDeskCustomerQueues.Add(cashDeskModel, new List<CustomerCharModel>(5));
            }

            var queueList = _cashDeskCustomerQueues[cashDeskModel];
            
            queueList.Add(customerCharModel);
        }

        private void OnShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            if (shopObjectModel.ShopObjectType.IsShelf())
            {
                _allShelfs.Add((ShelfModel)shopObjectModel);
            }
            else if(shopObjectModel.ShopObjectType == ShopObjectType.CashDesk)
            {
                var cashDeskModel = (CashDeskModel)shopObjectModel;
                
                _allCashDesks.Add(cashDeskModel);
                _cashDeskCustomerQueues.Add(cashDeskModel, new List<CustomerCharModel>());
            }

            UpdateMaxCustomersAmount();
        }

        private void PopulateShelfs()
        {
            var allShelfs = _shopModel.ShopObjects.Values
                .Where(o => o.ShopObjectType.IsShelf())
                .Cast<ShelfModel>()
                .ToArray();
            
            _allShelfs.AddRange(allShelfs);
        }
        
        private void PopulateCashDesks()
        {
            var allCashDesks = _shopModel.ShopObjects.Values
                .Where(o => o.ShopObjectType == ShopObjectType.CashDesk)
                .Cast<CashDeskModel>()
                .ToArray();
            
            _allCashDesks.AddRange(allCashDesks);
        }
    }
}