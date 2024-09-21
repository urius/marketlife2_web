using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using Model;
using Model.Popups;
using UnityEngine;
using Utils;
using View.UI.Popups.InteriorPopup;

namespace View.UI.Popups
{
    public class UIInteriorPopupMediator : MediatorWithModelBase<InteriorPopupViewModel>
    {
        private const int ColumnsCount = 3;

        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        
        private UITabbedContentPopup _popupView;
        private Vector2 _itemSize;
        private PlayerModel _playerModel;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            
            var itemSettings = GetComponentInPrefab<UIInteriorPopupItemView>(PrefabKey.UIInteriorPopupItem); 
            _itemSize = itemSettings.Size;
            
            _popupView = InstantiatePrefab<UITabbedContentPopup>(PrefabKey.UITabbedContentPopup);

            ShowContent(TargetModel.WallItemViewModels, InteriorItemType.Wall);
        }

        private void ShowContent(IReadOnlyList<InteriorPopupViewModelItemBase> viewModels,
            InteriorItemType interiorItemType)
        {
            _popupView.ClearContent();
            _popupView.ResetContentPosition();
            
            for (var i = 0; i < viewModels.Count; i++)
            {
                var itemView = InstantiatePrefab<UIInteriorPopupItemView>(PrefabKey.UIInteriorPopupItem, _popupView.ContentTransform);

                SetupItemView(itemView, viewModels[i], interiorItemType);
                
                var position = SetItemPosition(itemView, i);
                
                var newContentHeight = -position.y + _itemSize.y;
                _popupView.SetContentHeight(newContentHeight);
            }
        }

        private void SetupItemView(
            UIInteriorPopupItemView itemView, InteriorPopupViewModelItemBase itemViewModel, InteriorItemType interiorItemType)
        {
            var isUnlockedByLevel = itemViewModel.UnlockLevel <= _playerModel.Level;
            
            itemView.SetLockVisibility(!isUnlockedByLevel);
            itemView.SetButtonInteractable(isUnlockedByLevel && itemViewModel.IsChosen == false);

            switch (interiorItemType)
            {
                case InteriorItemType.Wall:
                {
                    var wallItemViewModel = (InteriorPopupViewModelWallItem)itemViewModel;
                    var sprite = _spritesHolderSo.GetWallSpriteByKey(wallItemViewModel.WallType);
                    itemView.SetItemSprite(sprite);
                    break;
                }
                case InteriorItemType.Floor:
                {
                    var floorItemViewModel = (InteriorPopupViewModelFloorItem)itemViewModel;
                    var sprite = _spritesHolderSo.GetFloorSpriteByKey(floorItemViewModel.FloorType);
                    itemView.SetItemSprite(sprite);
                    break;
                }
            }

            if (itemViewModel.IsBought)
            {
                itemView.SetButtonText(_localizationProvider.GetLocale(Constants.LocalizationChooseMessageKey));
            }
            else if (isUnlockedByLevel)
            {
                var buyCost = interiorItemType == InteriorItemType.Wall
                    ? InteriorCostHelper.GetWallCostForLevel(_playerModel.Level)
                    : InteriorCostHelper.GetFloorCostForLevel(_playerModel.Level);

                itemView.SetButtonText(itemViewModel.IsChosen
                    ? _localizationProvider.GetLocale(Constants.LocalizationChosenMessageKey)
                    : $"{FormattingHelper.ToMoneyWithIconText2Format(buyCost)}\n{_localizationProvider.GetLocale(Constants.LocalizationBuyMessageKey)}");
            }
            else
            {
                itemView.SetButtonText($"{Constants.TextIconStar} {_localizationProvider.GetLocale(Constants.LocalizationKeyLevel)} {itemViewModel.UnlockLevel}");
            }
        }

        private Vector2 SetItemPosition(UIInteriorPopupItemView item, int itemIndex)
        {
            var position = new Vector2Int(itemIndex % ColumnsCount, -itemIndex / ColumnsCount) * _itemSize;

            item.SetPosition(position);
            
            return position;
        }

        protected override void UnmediateInternal()
        {
            Destroy(_popupView);
            _popupView = null;
        }
        
        private enum InteriorItemType
        {
            Undefined,
            Wall,
            Floor,
        }
    }
}