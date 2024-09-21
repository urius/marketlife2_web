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

            var popupViewModel = new InteriorPopupViewModel(floorItemViewModels, wallItemViewModels);
            
            _popupViewModelsHolder.AddPopup(popupViewModel);
        }

        private IEnumerable<InteriorPopupViewModelFloorItem> GetFloorViewModels()
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var shopModel = playerModel.ShopModel;
            var currentLevel = playerModel.Level;
            
            var floorItemsOnLevel = _interiorDataProvider.GetFloorItemsByLevel(currentLevel);
            var unlockedFloorItemsOnLevel = playerModel.UnlockedFloors;
            var floorItemsOnNextLevel = _interiorDataProvider.GetFloorItemsForNextLevel(currentLevel);

            var itemsToShow = new LinkedList<InteriorPopupViewModelFloorItem>();

            foreach (var floorItem in floorItemsOnLevel)
            {
                var isBought = unlockedFloorItemsOnLevel.Contains(floorItem.FloorType);
                var isChosen = floorItem.FloorType == shopModel.FloorsType;
                itemsToShow.AddLast(
                    new InteriorPopupViewModelFloorItem(floorItem.Level, isBought, isChosen, floorItem.FloorType));
            }

            foreach (var lockedByLevelFloorItem in floorItemsOnNextLevel)
            {
                itemsToShow.AddLast(
                    new InteriorPopupViewModelFloorItem(
                        lockedByLevelFloorItem.Level, isBought: false, isChosen: false,
                        lockedByLevelFloorItem.FloorType));
            }

            return itemsToShow;
        }
        
        private IEnumerable<InteriorPopupViewModelWallItem> GetWallViewModels()
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var shopModel = playerModel.ShopModel;
            var currentLevel = playerModel.Level;

            var wallItemsOnLevel = _interiorDataProvider.GetWallItemsByLevel(currentLevel);
            var unlockedWallItemsOnLevel = playerModel.UnlockedWalls;
            var wallItemsOnNextLevel = _interiorDataProvider.GetWallItemsForNextLevel(currentLevel);

            var itemsToShow = new LinkedList<InteriorPopupViewModelWallItem>();

            foreach (var wallItem in wallItemsOnLevel)
            {
                var isBought = unlockedWallItemsOnLevel.Contains(wallItem.WallType);
                var isChosen = wallItem.WallType == shopModel.WallsType;
                itemsToShow.AddLast(
                    new InteriorPopupViewModelWallItem(wallItem.Level, isBought, isChosen, wallItem.WallType));
            }

            foreach (var lockedByLevelWallItem in wallItemsOnNextLevel)
            {
                itemsToShow.AddLast(
                    new InteriorPopupViewModelWallItem(
                        lockedByLevelWallItem.Level, isBought: false, isChosen: false,
                        lockedByLevelWallItem.WallType));
            }

            return itemsToShow;
        }
    }
}