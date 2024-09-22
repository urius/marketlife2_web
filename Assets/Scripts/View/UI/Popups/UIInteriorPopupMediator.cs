using System;
using System.Collections.Generic;
using Data;
using Events;
using Holders;
using Infra.EventBus;
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
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();

        private readonly Dictionary<UIInteriorPopupItemView, InteriorPopupItemViewModelBase> _viewModelByView = new();
        
        private UITabbedContentPopup _popupView;
        private Vector2 _itemSize;
        private PlayerModel _playerModel;
        private InteriorItemType _currentShowingInteriorItemType;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;

            SetPrefabCacheCapacity(
                PrefabKey.UIInteriorPopupItem,
                Math.Max(TargetModel.WallItemViewModels.Count, TargetModel.FloorItemViewModels.Count));
            
            var itemSettings = GetComponentInPrefab<UIInteriorPopupItemView>(PrefabKey.UIInteriorPopupItem); 
            _itemSize = itemSettings.Size;
            
            _popupView = InstantiatePrefab<UITabbedContentPopup>(PrefabKey.UITabbedContentPopup);
            
            _popupView.SetTitleText(_localizationProvider.GetLocale(Constants.LocalizationInteriorPopupTitleKey));

            ShowWalls();
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            RemoveItemViews();
            
            Destroy(_popupView);
            _popupView = null;
            
            ClearCache(PrefabKey.UIInteriorPopupItem);
        }

        private void Subscribe()
        {
            TargetModel.ItemBought += OnItemBought;
            TargetModel.ItemChosen += OnItemChosen;
        }

        private void Unsubscribe()
        {
            TargetModel.ItemBought -= OnItemBought;
            TargetModel.ItemChosen -= OnItemChosen;
        }

        private void OnItemBought(InteriorPopupItemViewModelBase itemViewModel)
        {
            UpdateItemViewStates();
        }

        private void OnItemChosen(InteriorPopupItemViewModelBase itemViewModel)
        {
            UpdateItemViewStates();
        }

        private void UpdateItemViewStates()
        {
            foreach (var (view, viewModel) in _viewModelByView)
            {
                UpdateItemViewState(view, viewModel, _currentShowingInteriorItemType);
            }
        }

        private void ShowWalls()
        {
            ShowContent(TargetModel.WallItemViewModels, InteriorItemType.Wall);
        }

        private void ShowContent(IReadOnlyList<InteriorPopupItemViewModelBase> viewModels,
            InteriorItemType interiorItemType)
        {
            _currentShowingInteriorItemType = interiorItemType;
            
            RemoveItemViews();
            _popupView.ResetContentPosition();
            
            for (var i = 0; i < viewModels.Count; i++)
            {
                var itemView = GetFromCache<UIInteriorPopupItemView>(PrefabKey.UIInteriorPopupItem, _popupView.ContentTransform);

                var itemViewModel = viewModels[i];
                SetupItemView(itemView, itemViewModel, _currentShowingInteriorItemType);
                
                var position = SetItemPosition(itemView, i);
                var newContentHeight = -position.y + _itemSize.y;
                _popupView.SetContentHeight(newContentHeight);
                
                _viewModelByView[itemView] = itemViewModel;
                SubscribeOnItemView(itemView);
            }
        }

        private void RemoveItemViews()
        {
            foreach (var itemView in _viewModelByView.Keys)
            {
                UnsubscribeFromItemView(itemView);
                ReturnToCache(itemView.gameObject);
            }
            
            _viewModelByView.Clear();
        }

        private void SetupItemView(
            UIInteriorPopupItemView itemView, InteriorPopupItemViewModelBase itemViewModel, InteriorItemType interiorItemType)
        {
            switch (interiorItemType)
            {
                case InteriorItemType.Wall:
                {
                    var wallItemViewModel = (InteriorPopupWallItemViewModel)itemViewModel;
                    var sprite = _spritesHolderSo.GetWallSpriteByKey(wallItemViewModel.WallType);
                    itemView.SetItemSprite(sprite);
                    break;
                }
                case InteriorItemType.Floor:
                {
                    var floorItemViewModel = (InteriorPopupFloorItemViewModel)itemViewModel;
                    var sprite = _spritesHolderSo.GetFloorSpriteByKey(floorItemViewModel.FloorType);
                    itemView.SetItemSprite(sprite);
                    break;
                }
            }

            UpdateItemViewState(itemView, itemViewModel, interiorItemType);
        }

        private void UpdateItemViewState(UIInteriorPopupItemView itemView, InteriorPopupItemViewModelBase itemViewModel,
            InteriorItemType interiorItemType)
        {
            var isUnlockedByLevel = itemViewModel.UnlockLevel <= _playerModel.Level;

            itemView.SetLockVisibility(!isUnlockedByLevel);
            itemView.SetButtonInteractable(isUnlockedByLevel && itemViewModel.IsChosen == false);

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
                itemView.SetButtonText(
                    $"{Constants.TextIconStar} {_localizationProvider.GetLocale(Constants.LocalizationKeyLevel)} {itemViewModel.UnlockLevel}");
            }
        }

        private void SubscribeOnItemView(UIInteriorPopupItemView itemView)
        {
            itemView.ButtonClicked += OnItemButtonClicked;
        }

        private void UnsubscribeFromItemView(UIInteriorPopupItemView itemView)
        {
            itemView.ButtonClicked -= OnItemButtonClicked;
        }

        private void OnItemButtonClicked(UIInteriorPopupItemView itemView)
        {
            var itemViewModel = _viewModelByView[itemView];
            
            _eventBus.Dispatch(new UIInteriorPopupItemClickedEvent(itemViewModel));
        }

        private Vector2 SetItemPosition(UIInteriorPopupItemView item, int itemIndex)
        {
            var position = new Vector2Int(itemIndex % ColumnsCount, -itemIndex / ColumnsCount) * _itemSize;

            item.SetPosition(position);
            
            return position;
        }

        private enum InteriorItemType
        {
            Undefined,
            Wall,
            Floor,
        }
    }
}