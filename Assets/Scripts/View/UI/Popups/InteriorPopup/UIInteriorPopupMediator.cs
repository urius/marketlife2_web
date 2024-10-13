using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Data.Internal;
using Events;
using Extensions;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.Popups;
using Tools.AudioManager;
using Utils;
using View.UI.Popups.TabbedContentPopup;

namespace View.UI.Popups.InteriorPopup
{
    public class UIInteriorPopupMediator : MediatorWithModelBase<InteriorPopupViewModel>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();

        private readonly Dictionary<UIInteriorPopupItemView, PopupItemViewModelBase> _viewModelByView = new();
        private readonly InteriorItemType[] _tabTypes = { InteriorItemType.Wall, InteriorItemType.Floor };
        
        private UITabbedContentPopup _popupView;
        private PlayerModel _playerModel;
        private InteriorItemType _currentShowingInteriorItemType;
        private PlayerUIFlagsModel _uiFlagsModel;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            _uiFlagsModel = _playerModel.UIFlagsModel;

            SetPrefabCacheCapacity(
                PrefabKey.UIInteriorPopupItem,
                Math.Max(TargetModel.WallItemViewModels.Count, TargetModel.FloorItemViewModels.Count));
            
            _popupView = InstantiatePrefab<UITabbedContentPopup>(PrefabKey.UITabbedContentPopup);

            _popupView.Setup(columnsCount: 3, popupWidth: 570, popupHeight: 500);
            _popupView.SetTitleText(_localizationProvider.GetLocale(Constants.LocalizationInteriorPopupTitleKey));
            
            InitTabs();

            Subscribe();

            ShowTab(0);

            Appear().Forget();
        }

        protected override void UnmediateInternal()
        {
            _eventBus.Dispatch(new RequestGamePauseEvent(nameof(UIInteriorPopupMediator), false));
            
            Unsubscribe();
            
            RemoveItemViews();
            
            Destroy(_popupView);
            _popupView = null;
            
            ClearCache(PrefabKey.UIInteriorPopupItem);
        }

        private async UniTaskVoid Appear()
        {
            _eventBus.Dispatch(new RequestGamePauseEvent(nameof(UIInteriorPopupMediator), true));
            
            _audioPlayer.PlaySound(SoundIdKey.PopupOpen);
            
            await _popupView.AppearAsync();
        }

        private void ShowTab(int tabIndex)
        {
            var tabType = _tabTypes[tabIndex];
            
            switch (tabType)
            {
                case InteriorItemType.Wall:
                    ShowContent(TargetModel.WallItemViewModels, InteriorItemType.Wall);
                    break;
                case InteriorItemType.Floor:
                    ShowContent(TargetModel.FloorItemViewModels, InteriorItemType.Floor);
                    break;
            }

            _popupView.SetSelectedTab(tabIndex);
            
            UpdateNewNotificationsOnTabs();
            
            _eventBus.Dispatch(new UIInteriorPopupTabShownEvent(tabType));
        }

        private void InitTabs()
        {
            foreach (var tabType in _tabTypes)
            {
                var tabTitleKey = tabType switch
                {
                    InteriorItemType.Wall => Constants.LocalizationInteriorPopupWallsTabTitleKey,
                    InteriorItemType.Floor => Constants.LocalizationInteriorPopupFloorsTabTitleKey,
                    _ => string.Empty
                };
                _popupView.AddTab(_localizationProvider.GetLocale(tabTitleKey));
            }
        }

        private void Subscribe()
        {
            TargetModel.ItemBought += OnItemBought;
            TargetModel.ItemChosen += OnItemChosen;
            _uiFlagsModel.FloorsFlagChanged += OnFloorsFlagChanged;
            _uiFlagsModel.WallsFlagChanged += OnWallsFlagChanged;

            _popupView.TabButtonClicked += OnTabButtonClicked;
            _popupView.CloseButtonClicked += OnCloseButtonClicked;
        }

        private void Unsubscribe()
        {
            TargetModel.ItemBought -= OnItemBought;
            TargetModel.ItemChosen -= OnItemChosen;
            _uiFlagsModel.FloorsFlagChanged -= OnFloorsFlagChanged;
            _uiFlagsModel.WallsFlagChanged -= OnWallsFlagChanged;
            
            _popupView.TabButtonClicked -= OnTabButtonClicked;
            _popupView.CloseButtonClicked -= OnCloseButtonClicked;
        }

        private void OnFloorsFlagChanged(bool isEnabled)
        {
            UpdateNewNotificationsOnTabs();
        }

        private void OnWallsFlagChanged(bool isEnabled)
        {
            UpdateNewNotificationsOnTabs();
        }

        private void UpdateNewNotificationsOnTabs()
        {
            var floorTabIndex = Array.IndexOf(_tabTypes, InteriorItemType.Floor);
            _popupView.SetTabNewNotificationVisibility(floorTabIndex, _uiFlagsModel.HaveNewFloors);
            
            var wallTabIndex = Array.IndexOf(_tabTypes, InteriorItemType.Wall);
            _popupView.SetTabNewNotificationVisibility(wallTabIndex, _uiFlagsModel.HaveNewWalls);
        }

        private void OnCloseButtonClicked()
        {
            DisappearAndRequestUnmediate().Forget();
        }

        private async UniTaskVoid DisappearAndRequestUnmediate()
        {
            _eventBus.Dispatch(new RequestGamePauseEvent(nameof(UIInteriorPopupMediator), false));
            
            _audioPlayer.PlaySound(SoundIdKey.PopupClose);

            await _popupView.DisappearAsync();
            
            _eventBus.Dispatch(new UIRequestClosePopupEvent(TargetModel));
        }

        private void OnTabButtonClicked(int index)
        {
            _audioPlayer.PlaySound(SoundIdKey.Button_5);
            
            ShowTab(index);
        }

        private void OnItemBought(PopupItemViewModelBase itemViewModel)
        {
            UpdateItemViewStates();

            foreach (var kvp in _viewModelByView)
            {
                if (kvp.Value == itemViewModel)
                {
                    var text = _localizationProvider.GetLocale(Constants.LocalizationKeyBought);
                    _eventBus.Dispatch(new UIRequestFlyingTextEvent(text, kvp.Key.Button.transform.position, UIRequestFlyingTextColor.Green));
                    break;
                }
            }
        }

        private void OnItemChosen(PopupItemViewModelBase itemViewModel)
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

        private void ShowContent(IReadOnlyList<PopupItemViewModelBase> viewModels,
            InteriorItemType interiorItemType)
        {
            _currentShowingInteriorItemType = interiorItemType;
            
            RemoveItemViews();
            _popupView.ResetContentPosition();
            
            foreach (var itemViewModel in viewModels)
            {
                var itemView = GetFromCache<UIInteriorPopupItemView>(PrefabKey.UIInteriorPopupItem, _popupView.ContentTransform);

                SetupItemView(itemView, itemViewModel, _currentShowingInteriorItemType);
                
                _popupView.AddItem(itemView);
                
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
            
            _popupView.ClearContent();
            _viewModelByView.Clear();
        }

        private void SetupItemView(
            UIInteriorPopupItemView itemView, PopupItemViewModelBase itemViewModel, InteriorItemType interiorItemType)
        {
            switch (interiorItemType)
            {
                case InteriorItemType.Wall:
                {
                    var wallItemViewModel = (InteriorPopupWallItemViewModel)itemViewModel;
                    var sprite = _spritesHolderSo.GetWallSpriteByKey(wallItemViewModel.WallType);
                    itemView.SetItemSprite(sprite);
                    itemView.SetNewNotificationVisibility(wallItemViewModel.IsNew && _uiFlagsModel.HaveNewWalls);
                    break;
                }
                case InteriorItemType.Floor:
                {
                    var floorItemViewModel = (InteriorPopupFloorItemViewModel)itemViewModel;
                    var sprite = _spritesHolderSo.GetFloorSpriteByKey(floorItemViewModel.FloorType);
                    itemView.SetItemSprite(sprite);
                    itemView.SetNewNotificationVisibility(floorItemViewModel.IsNew && _uiFlagsModel.HaveNewFloors);
                    break;
                }
            }

            UpdateItemViewState(itemView, itemViewModel, interiorItemType);
        }

        private void UpdateItemViewState(UIInteriorPopupItemView itemView, PopupItemViewModelBase itemViewModel,
            InteriorItemType interiorItemType)
        {
            var itemLevel = itemViewModel.UnlockLevel;
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
                    ? CostHelper.GetWallCostForLevel(itemLevel)
                    : CostHelper.GetFloorCostForLevel(itemLevel);

                itemView.SetButtonText(itemViewModel.IsBought
                    ? _localizationProvider.GetLocale(Constants.LocalizationChosenMessageKey)
                    : $"{FormattingHelper.ToMoneyWithIconText2Format(buyCost)}\n{_localizationProvider.GetLocale(Constants.LocalizationBuyMessageKey)}");
            }
            else
            {
                itemView.SetButtonText(
                    $"{Constants.TextIconStar} {_localizationProvider.GetLocale(Constants.LocalizationKeyLevel)} {itemLevel}");
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
            _audioPlayer.PlaySound(SoundIdKey.Button_1);
            
            var itemViewModel = _viewModelByView[itemView];
            
            _eventBus.Dispatch(new UIInteriorPopupItemClickedEvent(itemViewModel));
        }
    }
}