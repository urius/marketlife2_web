using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
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

        private readonly Dictionary<UIInteriorPopupItemView, InteriorPopupItemViewModelBase> _viewModelByView = new();
        private readonly InteriorItemType[] _tabTypes = { InteriorItemType.Wall, InteriorItemType.Floor };
        
        private UITabbedContentPopup _popupView;
        private PlayerModel _playerModel;
        private InteriorItemType _currentShowingInteriorItemType;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;

            SetPrefabCacheCapacity(
                PrefabKey.UIInteriorPopupItem,
                Math.Max(TargetModel.WallItemViewModels.Count, TargetModel.FloorItemViewModels.Count));
            
            _popupView = InstantiatePrefab<UITabbedContentPopup>(PrefabKey.UITabbedContentPopup);

            _popupView.Setup(columnsCount: 3, popupWidth: 570, popupHeight: 500);
            _popupView.SetTitleText(_localizationProvider.GetLocale(Constants.LocalizationInteriorPopupTitleKey));
            
            InitTabs();

            ShowTab(0);

            AppearAndSubscribe().Forget();
        }

        private async UniTaskVoid AppearAndSubscribe()
        {
            _audioPlayer.PlaySound(SoundIdKey.PopupOpen);
            
            await _popupView.AppearAsync();

            Subscribe();
        }

        private void ShowTab(int tabIndex)
        {
            switch (_tabTypes[tabIndex])
            {
                case InteriorItemType.Wall:
                    ShowWalls();
                    break;
                case InteriorItemType.Floor:
                    ShowFloors();
                    break;
            }

            _popupView.SetSelectedTab(tabIndex);
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

            _popupView.TabButtonClicked += OnTabButtonClicked;
            _popupView.CloseButtonClicked += OnCloseButtonClicked;
        }

        private void Unsubscribe()
        {
            TargetModel.ItemBought -= OnItemBought;
            TargetModel.ItemChosen -= OnItemChosen;
            
            _popupView.TabButtonClicked -= OnTabButtonClicked;
            _popupView.CloseButtonClicked -= OnCloseButtonClicked;
        }

        private void OnCloseButtonClicked()
        {
            DisappearAndRequestUnmediate().Forget();
        }

        private async UniTaskVoid DisappearAndRequestUnmediate()
        {
            _audioPlayer.PlaySound(SoundIdKey.PopupClose);

            await _popupView.DisappearAsync();
            
            _eventBus.Dispatch(new UIRequestClosePopupEvent(TargetModel));
        }

        private void OnTabButtonClicked(int index)
        {
            ShowTab(index);
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

        private void ShowFloors()
        {
            ShowContent(TargetModel.FloorItemViewModels, InteriorItemType.Floor);
        }

        private void ShowContent(IReadOnlyList<InteriorPopupItemViewModelBase> viewModels,
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

                itemView.SetButtonText(itemViewModel.IsBought
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
            _audioPlayer.PlaySound(SoundIdKey.Button_1);
            
            var itemViewModel = _viewModelByView[itemView];
            
            _eventBus.Dispatch(new UIInteriorPopupItemClickedEvent(itemViewModel));
        }

        private enum InteriorItemType
        {
            Undefined,
            Wall,
            Floor,
        }
    }
}