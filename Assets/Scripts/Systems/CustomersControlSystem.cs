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
using Model.People.States;
using Model.People.States.Customer;
using Model.ShopObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems
{
    public class CustomersControlSystem : BotCharsControlSystemBase
    {
        private const int SpawnY = -5;
        private const int DespawnY = -6;
        
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        
        private readonly List<CashDeskModel> _allCashDesks = new ();
        private readonly Dictionary<CashDeskModel, List<CustomerCharModel>> _cashDeskCustomerQueues = new ();

        private ShopModel _shopModel;
        private PlayerCharModel _playerCharModel;
        private CustomersModel _customersModel;
        private int _processNextIdleCustomerIndex = -1;
        
        public override void Start()
        {
            base.Start();
            
            _shopModel = _shopModelHolder.ShopModel;
            _customersModel = _shopModel.CustomersModel;
            _playerCharModel = _playerModelHolder.PlayerCharModel;

            PopulateCashDesks();
            UpdateMaxCustomersAmount();

            Subscribe();
        }

        public override void Stop()
        {
            Unsubscribe();
            
            base.Stop();
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
            var movingState = (BotCharMovingStateBase)customerModel.State;
            
            customerModel.IsStepInProgress = false;

            if (movingState.TargetCell == customerModel.CellCoords)
            {
                ProcessNextState(customerModel);
            }
            else
            {
                if (movingState.StateName == ShopCharStateName.CustomerMovingToCashDesk)
                {
                    var cashDesk = ((CustomerMovingToCashDeskState)movingState).TargetCashDesk;
                    var queueIndex = GetCustomerCashDeskQueueIndex(cashDesk, customerModel);
                    var queueTargetCell = GetCashDeskQueueCell(cashDesk, queueIndex);
                    
                    if (customerModel.CellCoords != queueTargetCell)
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
            
            return customer.CellCoords == queueTargetCell;
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

        protected override void BeforeChangeCell(BotCharModelBase customerCharModel, Vector2Int cell)
        {
            _customersModel.SetOwnedCell((CustomerCharModel)customerCharModel, cell);
        }

        private void ProcessNextState(CustomerCharModel model)
        {
            switch (model.State.StateName)
            {
                case ShopCharStateName.CustomerMovingToEnter:
                    SetMoveToRandomShelfState(model);
                    break;
                case ShopCharStateName.CustomerMovingToShelf:
                    var targetProduct = ((CustomerMovingToShelfState)model.State).TargetProduct;
                    if (targetProduct != ProductType.None)
                    {
                        var setStateSuccess = TrySetTakeProductState(model);
                        if (setStateSuccess == false)
                        {
                            setStateSuccess = TrySetMoveToShelfWithExactProductState(model, targetProduct);
                            if (setStateSuccess == false)
                            {
                                SetMoveToCashDeskOrExitState(model);
                            }
                        }
                    }
                    else
                    {
                        SetMoveToCashDeskOrExitState(model);
                    }
                    break;
                case ShopCharStateName.CustomerTakingProduct:
                    if (model.ProductsCount < Math.Min(_shopModel.CashDesks.Count, CustomerCharModel.MaxProductsAmount))
                    {
                        var rnd = Random.value;
                        if (rnd <= 0.6f)
                        {
                            if (TrySetTakeProductState(model))
                            {
                                break;
                            }
                        }
                        else if (rnd <= 0.8f)
                        {
                            SetMoveToRandomShelfState(model);
                            break;
                        }
                    }

                    SetMoveToCashDeskState(model);
                    break;
                case ShopCharStateName.CustomerMovingToCashDesk:
                    TrySetPayingState(((CustomerMovingToCashDeskState)model.State).TargetCashDesk, model);
                    break;
                case ShopCharStateName.CustomerPaying:
                    var targetCashDesk = ((CustomerPayingState)model.State).TargetCashDesk;
                    AddMoneyToCashDesk(targetCashDesk, model);
                    model.EnableBag();
                    SetMoveToExitState(model);
                    RemoveFromCashDeskQueue(targetCashDesk, model);
                    if (targetCashDesk.CashDeskStaffModel != null)
                    {
                        _eventBus.Dispatch(
                            new RequestCashDeskStaffAcceptingPayAnimationEvent(targetCashDesk.CashDeskStaffModel,
                                requestFinishAnimation: true));
                    }

                    break;
                case ShopCharStateName.CustomerMovingToExit:
                    SetMoveOutOfShopState(model);
                    break;
                case ShopCharStateName.CustomerMovingToDespawn:
                    _customersModel.RemoveCustomer(model);
                    break;
            }
        }

        private void SetMoveToCashDeskOrExitState(CustomerCharModel model)
        {
            if (model.HasProducts)
            {
                SetMoveToCashDeskState(model);
            }
            else
            {
                SetMoveToExitState(model);
            }
        }

        private void AddMoneyToCashDesk(CashDeskModel targetCashDesk, CustomerCharModel customerCharModel)
        {
            targetCashDesk.AddMoney(customerCharModel.ProductsCount);
        }

        private void RemoveFromCashDeskQueue(CashDeskModel cashDeskModel, CustomerCharModel customerCharModel)
        {
            var queue = _cashDeskCustomerQueues[cashDeskModel];
            var customerIndex = queue.IndexOf(customerCharModel);
            if (customerIndex > -1)
            {
                if (customerIndex != 0)
                {
                    Debug.LogWarning($"{nameof(RemoveFromCashDeskQueue)}: Requested remove not first customer in queue");
                }
                
                queue.RemoveAt(customerIndex);
            }
            else
            {
                Debug.LogWarning($"{nameof(RemoveFromCashDeskQueue)}: Requested customer not found in queue");
                Debug.Break();
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
            var closestDoor = GetClosestDoor(model.CellCoords);
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
            var closestDoor = GetClosestDoor(model.CellCoords);
            var targetPoint = new Vector2Int(closestDoor.Right, -2);

            model.SetMovingToExitState(targetPoint);

            MakeNextStep(model, targetPoint);
        }

        private void SetMoveToRandomShelfState(CustomerCharModel model)
        {
            var allShelfs = _shopModel.Shelfs;
            ShelfModel targetShelf = null;
            var rndInt = Random.Range(0, _shopModel.Shelfs.Count);
            var tempInt = 0;
            foreach (var shelfModel in allShelfs)
            {
                tempInt++;
                if (tempInt > rndInt
                    && shelfModel.HasProducts() 
                    && (targetShelf == null || Random.value < 0.5f))
                {
                    targetShelf = shelfModel;
                }
            }

            targetShelf ??= allShelfs[Random.Range(0, allShelfs.Count)];
            
            var targetProduct = targetShelf.GetRandomNotEmptyProductOrDefault();

            SetMoveToShelfState(model, targetProduct, targetShelf);
        }

        private bool TrySetMoveToShelfWithExactProductState(CustomerCharModel model, ProductType targetProduct)
        {
            var targetShelf = _shopModel.Shelfs.FirstOrDefault(s => s.HasProduct(targetProduct));

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
            ShelfModel targetShelfModel = null;
            ProductType targetProduct = ProductType.None;
            
            switch (model.State.StateName)
            {
                case ShopCharStateName.CustomerTakingProduct:
                {
                    var currentMoveToShelfState = (CustomerTakeProductFromShelfState)model.State;
                    targetShelfModel = currentMoveToShelfState.TargetShelfModel;
                    targetProduct = currentMoveToShelfState.ProductType;
                    break;
                }
                case ShopCharStateName.CustomerMovingToShelf:
                {
                    var currentMoveToShelfState = (CustomerMovingToShelfState)model.State;
                    targetShelfModel = currentMoveToShelfState.TargetShelf;
                    targetProduct = currentMoveToShelfState.TargetProduct;
                    break;
                }
            }

            if (targetProduct == ProductType.None || targetShelfModel == null) return false;
            
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

        private Vector2Int GetShelfBuyPoint(ShelfModel targetShelf)
        {
            var ownedCells = _ownedCellsDataHolder.GetShopObjectOwnedCells(targetShelf);
            var shelfCellIndex = Random.Range(0, ownedCells.Length);
            
            return ownedCells[shelfCellIndex] + new Vector2Int(0, 1);
        }

        protected override bool CanMakeStepTo(Vector2Int cell)
        {
            return base.CanMakeStepTo(cell)
                   && !_customersModel.HaveCustomerOnCell(cell);
        }

        private void UpdateMaxCustomersAmount()
        {
            var amount = _shopModel.CashDesks.Count * _shopModel.Size.y;
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
                && customer.State is BotCharMovingStateBase movingStateBase)
            {
                if (movingStateBase.StateName == ShopCharStateName.CustomerMovingToCashDesk)
                {
                    var moveToCashDeskState = (CustomerMovingToCashDeskState)movingStateBase;
                    var cashDesk = moveToCashDeskState.TargetCashDesk;
                    var currentQueueIndex = GetCustomerCashDeskQueueIndex(cashDesk, customer);
                    
                    if (currentQueueIndex == 0 && customer.CellCoords == GetCashDeskPayPoint(cashDesk))
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

                if (cashDesk.CashDeskStaffModel != null)
                {
                    _eventBus.Dispatch(new RequestCashDeskStaffAcceptingPayAnimationEvent(cashDesk.CashDeskStaffModel));
                }

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

            var allShelfs = _shopModel.Shelfs;

            if (_customersModel.SpawnCooldownSecondsLeft <= 0
                && _customersModel.CustomersAmount < _customersModel.MaxCustomersAmount
                && allShelfs.Count > 0
                && (allShelfs.Count > 10 || HaveShelfsWithProducts()))
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
            foreach (var shelf in _shopModel.Shelfs)
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
            if (shopObjectModel.ShopObjectType == ShopObjectType.CashDesk)
            {
                var cashDeskModel = (CashDeskModel)shopObjectModel;

                _allCashDesks.Add(cashDeskModel);
                _cashDeskCustomerQueues.Add(cashDeskModel, new List<CustomerCharModel>());
            }

            UpdateMaxCustomersAmount();
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