using System;
using System.Collections.Generic;
using Data;

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

        public event Action<PlayerMoneyEarnModifierModel> MoneyEarnModifierAdded;
        public event Action<PlayerMoneyEarnModifierModel> MoneyEarnModifierRemoved;
        
        public readonly ShopModel ShopModel;
        public readonly PlayerCharModel PlayerCharModel;
        public readonly LinkedList<int> PassedTutorialSteps = new();
        //
        public readonly LinkedList<TutorialStep> OpenTutorialSteps = new(); //not for save
        //
        
        private readonly List<WallType> _boughtWalls;
        private readonly List<FloorType> _boughtFloors;

        public readonly PlayerAudioSettingsModel AudioSettingsModel;
        public readonly PlayerUIFlagsModel UIFlagsModel;
        public readonly PlayerStatsModel StatsModel;

        public PlayerModel(ShopModel shopModel, int moneyAmount, int level, 
            int staffWorkTimeSeconds, WallType[] boughtWalls, FloorType[] boughtFloors,
            PlayerCharModel playerCharModel, IEnumerable<int> passedTutorialSteps,
            PlayerAudioSettingsModel audioSettingsModel, PlayerUIFlagsModel uiFlagsModel, PlayerStatsModel statsModel)
        {
            ShopModel = shopModel;
            MoneyAmount = moneyAmount;
            Level = level <= 0 ? 1 : level;
            StaffWorkTimeSeconds = staffWorkTimeSeconds;
            _boughtWalls = new List<WallType>(boughtWalls);
            AddBoughtWall(shopModel.WallsType);
            _boughtFloors = new List<FloorType>(boughtFloors);
            AddBoughtFloor(shopModel.FloorsType);
            PlayerCharModel = playerCharModel;
            AudioSettingsModel = audioSettingsModel;
            UIFlagsModel = uiFlagsModel;
            StatsModel = statsModel;

            if (passedTutorialSteps != null)
            {
                foreach (var s in passedTutorialSteps)
                {
                    PassedTutorialSteps.AddLast(s);
                }
            }
        }

        public IReadOnlyList<WallType> BoughtWalls => _boughtWalls;
        public IReadOnlyList<FloorType> BoughtFloors => _boughtFloors;
        public bool IsLevelProcessingActive { get; private set; }
        public int MoneyAmount { get; private set; }
        public int Level { get; private set; }
        public int StaffWorkTimeSeconds { get; private set; }
        public int LevelIndex => Level - 1;
        public PlayerMoneyEarnModifierModel MoneyEarnModifier { get; private set; }
    
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
            var resultDeltaMoney = MoneyEarnModifier?.Apply(deltaMoney) ?? deltaMoney;

            MoneyAmount += resultDeltaMoney;

            if (resultDeltaMoney > 0)
            {
                StatsModel.AddTotalMoneyEarned(resultDeltaMoney);
            }
            
            MoneyChanged?.Invoke(resultDeltaMoney);
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

        public void AddMoneyEarnModifier(PlayerMoneyEarnModifierModel modifier)
        {
            MoneyEarnModifier = modifier;
            MoneyEarnModifierAdded?.Invoke(modifier);
        }

        public void RemoveMoneyEarnModifier()
        {
            var modifier = MoneyEarnModifier;
            MoneyEarnModifier = null;
            MoneyEarnModifierRemoved?.Invoke(modifier);
        }
    }
}