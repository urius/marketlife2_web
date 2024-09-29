using System.Collections.Generic;
using System.Linq;
using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Model.Popups;

namespace Commands
{
    public class ShowInteriorPopupCommand : ICommand<UIInteriorButtonClickedEvent>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IPopupViewModelsHolder _popupViewModelsHolder = Instance.Get<IPopupViewModelsHolder>();
        private readonly IInteriorDataProvider _interiorDataProvider = Instance.Get<IInteriorDataProvider>();

        public void Execute(UIInteriorButtonClickedEvent e)
        {
            var floorItemViewModels = GetFloorViewModels();
            var wallItemViewModels = GetWallViewModels();

            var popupViewModel =
                new InteriorPopupViewModel(floorItemViewModels, wallItemViewModels, _playerModelHolder.PlayerModel);
            
            _popupViewModelsHolder.AddPopup(popupViewModel);
        }

        private IEnumerable<InteriorPopupFloorItemViewModel> GetFloorViewModels()
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var shopModel = playerModel.ShopModel;
            var currentLevel = playerModel.Level;
            
            var floorItemsOnLevel = _interiorDataProvider.GetFloorItemsByLevel(currentLevel);
            var boughtFloorItemsOnLevel = playerModel.BoughtFloors;
            var floorItemsOnNextLevel = _interiorDataProvider.GetFloorItemsForNextLevel(currentLevel);

            var itemsToShow = new LinkedList<InteriorPopupFloorItemViewModel>();

            foreach (var floorItem in floorItemsOnLevel)
            {
                var isBought = boughtFloorItemsOnLevel.Contains(floorItem.FloorType);
                var isChosen = floorItem.FloorType == shopModel.FloorsType;
                var isNew = floorItem.Level == currentLevel;
                itemsToShow.AddLast(
                    new InteriorPopupFloorItemViewModel(floorItem.Level, isBought, isChosen, floorItem.FloorType, isNew));
            }

            foreach (var lockedByLevelFloorItem in floorItemsOnNextLevel)
            {
                itemsToShow.AddLast(
                    new InteriorPopupFloorItemViewModel(
                        lockedByLevelFloorItem.Level, isBought: false, isChosen: false,
                        lockedByLevelFloorItem.FloorType, isNew: false));
            }

            return itemsToShow;
        }
        
        private IEnumerable<InteriorPopupWallItemViewModel> GetWallViewModels()
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var shopModel = playerModel.ShopModel;
            var currentLevel = playerModel.Level;

            var wallItemsOnLevel = _interiorDataProvider.GetWallItemsByLevel(currentLevel);
            var boughtWallItemsOnLevel = playerModel.BoughtWalls;
            var wallItemsOnNextLevel = _interiorDataProvider.GetWallItemsForNextLevel(currentLevel);

            var itemsToShow = new LinkedList<InteriorPopupWallItemViewModel>();

            foreach (var wallItem in wallItemsOnLevel)
            {
                var isBought = boughtWallItemsOnLevel.Contains(wallItem.WallType);
                var isChosen = wallItem.WallType == shopModel.WallsType;
                var isNew = wallItem.Level == currentLevel;
                itemsToShow.AddLast(
                    new InteriorPopupWallItemViewModel(wallItem.Level, isBought, isChosen, wallItem.WallType, isNew));
            }

            foreach (var lockedByLevelWallItem in wallItemsOnNextLevel)
            {
                itemsToShow.AddLast(
                    new InteriorPopupWallItemViewModel(
                        lockedByLevelWallItem.Level, isBought: false, isChosen: false,
                        lockedByLevelWallItem.WallType, isNew: false));
            }

            return itemsToShow;
        }
    }
}