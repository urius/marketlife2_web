using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Unity.VisualScripting;

namespace Model
{
    public class PlayerModel
    {
        public event Action<int> MoneyChanged;
        public event Action<int> LevelChanged;
        public event Action<int> InsufficientFunds;
        public event Action<bool> IsLevelProcessingActiveFlagUpdated;
        public event Action<TutorialStep> OpenTutorialStepAdded;
        public event Action<TutorialStep> OpenTutorialStepRemoved;
        public event Action<WallType> WallBought;
        public event Action<FloorType> FloorBought;
        
        public readonly ShopModel ShopModel;
        public readonly PlayerCharModel PlayerCharModel;
        public readonly LinkedList<int> PassedTutorialSteps = new();
        //
        public readonly LinkedList<TutorialStep> OpenTutorialSteps = new(); //not for save
        //
        
        private readonly List<WallType> _boughtWalls;
        private readonly List<FloorType> _boughtFloors;

        public PlayerModel(ShopModel shopModel, int moneyAmount, int level, 
            int staffWorkTimeSeconds, WallType[] unlockedWalls, FloorType[] unlockedFloors,
            PlayerCharModel playerCharModel, IEnumerable<TutorialStep> passedTutorialSteps)
        {
            ShopModel = shopModel;
            MoneyAmount = moneyAmount;
            Level = 3;//level <= 0 ? 1 : level;
            StaffWorkTimeSeconds = staffWorkTimeSeconds;
            _boughtWalls = new List<WallType>(unlockedWalls);
            AddBoughtWall(shopModel.WallsType);
            _boughtFloors = new List<FloorType>(unlockedFloors);
            AddBoughtFloor(shopModel.FloorsType);
            PlayerCharModel = playerCharModel;
            if (passedTutorialSteps != null)
            {
                PassedTutorialSteps.AddRange(passedTutorialSteps.Select(s => (int)s));
            }
        }

        public IReadOnlyList<WallType> BoughtWalls => _boughtWalls;
        public IReadOnlyList<FloorType> BoughtFloors => _boughtFloors;
        public bool IsLevelProcessingActive { get; private set; }
        public int MoneyAmount { get; private set; }
        public int Level { get; private set; }
        public int StaffWorkTimeSeconds { get; private set; }
        public int LevelIndex => Level - 1;

        public void AddBoughtWall(WallType newWallType)
        {
            if (_boughtWalls.Contains(newWallType)) return;
            
            _boughtWalls.Add(newWallType);
            
            WallBought?.Invoke(newWallType);
        }
        
        public void AddBoughtFloor(FloorType newFloorType)
        {
            if (_boughtFloors.Contains(newFloorType)) return;

            _boughtFloors.Add(newFloorType);
    
            FloorBought?.Invoke(newFloorType);
        }
        
        public void ChangeMoney(int deltaMoney)
        {
            MoneyAmount += deltaMoney;
            
            MoneyChanged?.Invoke(MoneyAmount);
        }

        public bool TrySpendMoney(int amount)
        {
            if (amount <= MoneyAmount)
            {
                ChangeMoney(-amount);
                
                return true;
            }

            InsufficientFunds?.Invoke(amount - MoneyAmount);
            
            return false;
        }
        
        public void SetLevel(int level)
        {
            Level = level;

            LevelChanged?.Invoke(Level);
        }

        public void SetIsLevelProcessingActive(bool isActive)
        {
            if (IsLevelProcessingActive == isActive) return;

            IsLevelProcessingActive = isActive;

            IsLevelProcessingActiveFlagUpdated?.Invoke(isActive);
        }

        public bool IsTutorialStepPassed(TutorialStep step)
        {
            return IsTutorialStepPassed((int)step);
        }
        
        public bool IsTutorialStepPassed(int step)
        {
            return PassedTutorialSteps.Contains(step);
        }

        public bool AddPassedTutorialStep(TutorialStep step)
        {
            var intStep = (int)step;
            if (PassedTutorialSteps.Contains(intStep))
            {
                return false;
            }

            PassedTutorialSteps.AddLast(intStep);
            
            return true;
        }

        public bool AddOpenTutorialStep(TutorialStep step)
        {
            if (OpenTutorialSteps.Contains(step))
            {
                return false;
            }

            OpenTutorialSteps.AddLast(step);

            OpenTutorialStepAdded?.Invoke(step);

            return true;
        }

        public bool RemoveOpenTutorialStep(TutorialStep step)
        {
            if (OpenTutorialSteps.Contains(step))
            {
                OpenTutorialSteps.Remove(step);
                
                OpenTutorialStepRemoved?.Invoke(step);
                
                return true;
            }

            return false;
        }
    }
}