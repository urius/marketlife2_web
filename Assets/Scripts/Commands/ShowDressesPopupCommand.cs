using System.Collections.Generic;
using System.Linq;
using Data;
using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Model.Popups;

namespace Commands
{
    public class ShowDressesPopupCommand : ICommand<UIDressesButtonClickedEvent>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IPlayerDressesDataProvider _playerDressesDataProvider = Instance.Get<IPlayerDressesDataProvider>();
        private readonly IPopupViewModelsHolder _popupViewModelsHolder = Instance.Get<IPopupViewModelsHolder>();

        public void Execute(UIDressesButtonClickedEvent e)
        {
            var playerCharModel = _playerModelHolder.PlayerCharModel;

            var topDresses = GetTopDressViewModels().ToArray();
            var bottomDresses = GetBottomDressViewModels().ToArray();
            var hairDresses = GetHairViewModels().ToArray();
            var glassesDresses = GetGlassesViewModels().ToArray();

            var viewModel = new PlayerDressesPopupViewModel(
                topDresses,
                bottomDresses,
                hairDresses,
                glassesDresses,
                playerCharModel.DressesModel);
            
            _popupViewModelsHolder.AddPopup(viewModel);
        }

        private IEnumerable<DressesPopupItemViewModel> GetTopDressViewModels()
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var dressesModel = _playerModelHolder.PlayerCharModel.DressesModel;

            var currentItemTypeOnPlayer = dressesModel.TopDressType;
            var allItemsOnLevel = _playerDressesDataProvider.GetTopBodyItemsByLevel(playerModel.Level);
            var boughtItemsOnLevel = dressesModel.BoughtTopDresses;
            var itemsOnNextLevel = _playerDressesDataProvider.GetTopBodyItemsForNextLevel(playerModel.Level);

            return GetTabItemViewModels(
                PlayerDressesPopupTabType.TopDresses,
                currentItemTypeOnPlayer,
                allItemsOnLevel,
                boughtItemsOnLevel,
                itemsOnNextLevel);
        }
        
        private IEnumerable<DressesPopupItemViewModel> GetBottomDressViewModels()
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var dressesModel = _playerModelHolder.PlayerCharModel.DressesModel;

            var currentItemTypeOnPlayer = dressesModel.BottomDressType;
            var allItemsOnLevel = _playerDressesDataProvider.GetBottomBodyItemsByLevel(playerModel.Level);
            var boughtItemsOnLevel = dressesModel.BoughtBottomDresses;
            var itemsOnNextLevel = _playerDressesDataProvider.GetBottomBodyItemsForNextLevel(playerModel.Level);

            return GetTabItemViewModels(
                PlayerDressesPopupTabType.BottomDresses,
                currentItemTypeOnPlayer,
                allItemsOnLevel,
                boughtItemsOnLevel,
                itemsOnNextLevel);
        }
        
        private IEnumerable<DressesPopupItemViewModel> GetHairViewModels()
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var dressesModel = _playerModelHolder.PlayerCharModel.DressesModel;

            var currentItemTypeOnPlayer = dressesModel.HairType;
            var allItemsOnLevel = _playerDressesDataProvider.GetHairItemsByLevel(playerModel.Level);
            var boughtItemsOnLevel = dressesModel.BoughtHairs;
            var itemsOnNextLevel = _playerDressesDataProvider.GetHairItemsForNextLevel(playerModel.Level);

            return GetTabItemViewModels(
                PlayerDressesPopupTabType.Hairs,
                currentItemTypeOnPlayer,
                allItemsOnLevel,
                boughtItemsOnLevel,
                itemsOnNextLevel);
        }
        
        private IEnumerable<DressesPopupItemViewModel> GetGlassesViewModels()
        {
            var playerModel = _playerModelHolder.PlayerModel;
            var dressesModel = _playerModelHolder.PlayerCharModel.DressesModel;

            var currentItemTypeOnPlayer = dressesModel.GlassesType;
            var allItemsOnLevel = _playerDressesDataProvider.GetGlassItemsByLevel(playerModel.Level);
            var boughtItemsOnLevel = dressesModel.BoughtGlasses;
            var itemsOnNextLevel = _playerDressesDataProvider.GetGlassItemsForNextLevel(playerModel.Level);

            return GetTabItemViewModels(
                PlayerDressesPopupTabType.Glasses,
                currentItemTypeOnPlayer,
                allItemsOnLevel,
                boughtItemsOnLevel,
                itemsOnNextLevel);
        }

        private IEnumerable<DressesPopupItemViewModel> GetTabItemViewModels(
            PlayerDressesPopupTabType tabType,
            ManSpriteType currentItemTypeOnPlayer, IEnumerable<PlayerDressItemData> allItemsOnLevel, 
            IReadOnlyList<ManSpriteType> boughtItemsOnLevel, IEnumerable<PlayerDressItemData> itemsOnNextLevel)
        {
            var currentLevel = _playerModelHolder.PlayerModel.Level;
            var itemsToShow = new LinkedList<DressesPopupItemViewModel>();

            foreach (var item in allItemsOnLevel)
            {
                var isBought = boughtItemsOnLevel.Contains(item.PrimarySpriteKey);
                var isChosen = item.PrimarySpriteKey == currentItemTypeOnPlayer;
                var isNew = item.Level == currentLevel;
                itemsToShow.AddLast(
                    new DressesPopupItemViewModel(
                        tabType,
                        item.PrimarySpriteKey,
                        item.SecondarySpriteKey,
                        item.Level,
                        isBought,
                        isChosen,
                        isNew));
            }

            foreach (var item in itemsOnNextLevel)
            {
                itemsToShow.AddLast(
                    new DressesPopupItemViewModel(
                        tabType,
                        item.PrimarySpriteKey,
                        item.SecondarySpriteKey,
                        item.Level,
                        isBought: false,
                        isChosen: false,
                        isNew: false));
            }

            return itemsToShow;
        }
    }
}