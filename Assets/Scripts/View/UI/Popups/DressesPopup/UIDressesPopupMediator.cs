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
using UnityEngine;
using Utils;
using View.UI.Popups.TabbedContentPopup;

namespace View.UI.Popups.DressesPopup
{
    public class UIDressesPopupMediator : MediatorWithModelBase<PlayerDressesPopupViewModel>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly SpritesHolderSo _spritesHolder = Instance.Get<SpritesHolderSo>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private readonly Dictionary<UIDressesPopupItemView, DressesPopupItemViewModel> _viewModelByView = new();
        
        private UITabbedContentPopup _popupView;
        private PlayerModel _playerModel;
        private PlayerDressesModel _dressesModel;
        private PlayerUIFlagsModel _uiFlagsModel;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            _uiFlagsModel = _playerModel.UIFlagsModel;
            _dressesModel = _playerModel.PlayerCharModel.DressesModel;
            
            SetPrefabCacheCapacity(
                PrefabKey.UIDressesPopupItem,
                GetMaxOfFourInts(TargetModel.TopDressItemViewModels.Count, TargetModel.BottomDressItemViewModels.Count,
                    TargetModel.HairItemViewModels.Count, TargetModel.GlassesItemViewModels.Count));

            _popupView = InstantiatePrefab<UITabbedContentPopup>(PrefabKey.UITabbedContentPopup);

            _popupView.Setup(columnsCount: 4, popupWidth: 670, popupHeight: 500);
            _popupView.SetTitleText(_localizationProvider.GetLocale(Constants.LocalizationDressesPopupTitleKey));

            InitTabs();

            Subscribe();
            
            ShowCurrentTab();
            
            Appear().Forget();
        }


        protected override void UnmediateInternal()
        {
            _eventBus.Dispatch(new RequestGamePauseEvent(nameof(UIDressesPopupMediator), false));

            Unsubscribe();

            RemoveItemViews();
            
            Destroy(_popupView);
            _popupView = null;
            
            ClearCache(PrefabKey.UIDressesPopupItem);
        }

        private void Subscribe()
        {
            _uiFlagsModel.DressesFlagChanged += DressesFlagChanged;
            
            TargetModel.TabChanged += OnTabChanged;
            TargetModel.ItemBought += OnItemBought;
            TargetModel.ItemChosen += OnItemChosen;
            
            _popupView.TabButtonClicked += OnTabButtonClicked;
            _popupView.CloseButtonClicked += OnCloseButtonClicked;
        }

        private void Unsubscribe()
        {
            _uiFlagsModel.DressesFlagChanged -= DressesFlagChanged;
            
            TargetModel.TabChanged -= OnTabChanged;
            TargetModel.ItemBought -= OnItemBought;
            TargetModel.ItemChosen -= OnItemChosen;
            
            _popupView.TabButtonClicked -= OnTabButtonClicked;
            _popupView.CloseButtonClicked -= OnCloseButtonClicked;
        }

        private void OnItemBought(DressesPopupItemViewModel viewModel)
        {
            UpdateItemViewStates();
            
            foreach (var kvp in _viewModelByView)
            {
                if (kvp.Value == viewModel)
                {
                    var text = _localizationProvider.GetLocale(Constants.LocalizationKeyBought);
                    _eventBus.Dispatch(new UIRequestFlyingTextEvent(text, kvp.Key.Button.transform.position, UIRequestFlyingTextColor.Green));
                    break;
                }
            }
        }

        private void OnItemChosen(DressesPopupItemViewModel viewModel)
        {
            UpdateItemViewStates();
        }
        
        private void UpdateItemViewStates()
        {
            foreach (var (view, viewModel) in _viewModelByView)
            {
                UpdateItemViewState(view, viewModel, TargetModel.CurrentTabType);
            }
        }

        private void OnTabButtonClicked(int tabIndex)
        {
            TargetModel.NotifyTabClicked(tabIndex);
        }

        private void OnTabChanged(PlayerDressesPopupTabType tabType)
        {
            ShowCurrentTab();
        }

        private void ShowCurrentTab()
        {
            var tabIndex = Array.IndexOf(TargetModel.TabTypes, TargetModel.CurrentTabType);
            var tabType = TargetModel.CurrentTabType;
            
            switch (tabType)
            {
                case PlayerDressesPopupTabType.TopDresses:
                    ShowContent(TargetModel.TopDressItemViewModels, tabType);
                    break;
                case PlayerDressesPopupTabType.BottomDresses:
                    ShowContent(TargetModel.BottomDressItemViewModels, tabType);
                    break;
                case PlayerDressesPopupTabType.Hairs:
                    ShowContent(TargetModel.HairItemViewModels, tabType);
                    break;
                case PlayerDressesPopupTabType.Glasses:
                    ShowContent(TargetModel.GlassesItemViewModels, tabType);
                    break;
            }

            _popupView.SetSelectedTab(tabIndex);
            
            UpdateNewNotificationsOnTabs();
            
            _eventBus.Dispatch(new UIDressesPopupTabShownEvent(tabType));
        }

        private void ShowContent(IReadOnlyList<DressesPopupItemViewModel> viewModels, PlayerDressesPopupTabType tabType)
        {
            RemoveItemViews();
            _popupView.ResetContentPosition();
            
            foreach (var itemViewModel in viewModels)
            {
                var itemView = GetFromCache<UIDressesPopupItemView>(PrefabKey.UIDressesPopupItem, _popupView.ContentTransform);

                SetupItemView(itemView, itemViewModel, tabType);
                
                _popupView.AddItem(itemView);
                
                _viewModelByView[itemView] = itemViewModel;
                SubscribeOnItemView(itemView);
            }
        }

        private void SetupItemView(
            UIDressesPopupItemView itemView, DressesPopupItemViewModel itemViewModel, PlayerDressesPopupTabType tabType)
        {
            var sprite = _spritesHolder.GetManSpriteByKey(itemViewModel.PrimarySpriteType);
            
            switch (tabType)
            {
                case PlayerDressesPopupTabType.TopDresses:
                    var sprite2 = _spritesHolder.GetManSpriteByKey(itemViewModel.SecondarySpriteType);
                    itemView.SetupTopDress(sprite, sprite2);
                    itemView.SetNewNotificationVisibility(itemViewModel.IsNew && _uiFlagsModel.HaveNewTopDresses);
                    break;
                case PlayerDressesPopupTabType.BottomDresses:
                    itemView.SetupBottomDress(sprite);
                    itemView.SetNewNotificationVisibility(itemViewModel.IsNew && _uiFlagsModel.HaveNewBottomDresses);
                    break;
                case PlayerDressesPopupTabType.Hairs:
                    itemView.SetupGlasses(null);
                    sprite = GetHairSprite(itemViewModel.PrimarySpriteType);
                    itemView.SetupHair(sprite);
                    itemView.SetNewNotificationVisibility(itemViewModel.IsNew && _uiFlagsModel.HaveNewHairs);
                    break;
                case PlayerDressesPopupTabType.Glasses:
                    itemView.SetupGlasses(sprite);
                    sprite = GetHairSprite(_dressesModel.HairType);
                    itemView.SetupHair(sprite);
                    itemView.SetNewNotificationVisibility(itemViewModel.IsNew && _uiFlagsModel.HaveNewGlasses);
                    break;
            }
            
            UpdateItemViewState(itemView, itemViewModel, tabType);
        }

        private Sprite GetHairSprite(ManSpriteType hairSpriteType)
        {
            return _spritesHolder.GetManSpriteByKey((ManSpriteType)((int)hairSpriteType + Constants.ManUISpriteKeyOffset));
        }

        private void UpdateItemViewState(UIDressesPopupItemView itemView, DressesPopupItemViewModel itemViewModel,
            PlayerDressesPopupTabType tabType)
        {
            var itemLevel = itemViewModel.UnlockLevel;
            var isUnlockedByLevel = itemLevel <= _playerModel.Level;

            itemView.SetLockVisibility(!isUnlockedByLevel);
            itemView.SetButtonInteractable(isUnlockedByLevel && itemViewModel.IsChosen == false);

            if (itemViewModel.IsBought)
            {
                itemView.SetButtonText(_localizationProvider.GetLocale(Constants.LocalizationChooseMessageKey));
            }
            else if (isUnlockedByLevel)
            {
                var buyCost =  tabType switch
                {
                    PlayerDressesPopupTabType.TopDresses => CostHelper.GetTopDressCostForLevel(itemLevel),
                    PlayerDressesPopupTabType.BottomDresses => CostHelper.GetBottomDressCostForLevel(itemLevel),
                    PlayerDressesPopupTabType.Hairs => CostHelper.GetHairCostForLevel(itemLevel),
                    PlayerDressesPopupTabType.Glasses => CostHelper.GetGlassesCostForLevel(itemLevel),
                    _ => 1
                }

                ;
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

        private async UniTaskVoid Appear()
        {
            _eventBus.Dispatch(new RequestGamePauseEvent(nameof(UIDressesPopupMediator), true));
            
            _audioPlayer.PlaySound(SoundIdKey.PopupOpen);
            
            await _popupView.AppearAsync();
        }

        private void InitTabs()
        {
            foreach (var tabType in TargetModel.TabTypes)
            {
                var tabTitleKey = tabType switch
                {
                    PlayerDressesPopupTabType.TopDresses => Constants.LocalizationDressesPopupTopDressTabTitleKey,
                    PlayerDressesPopupTabType.BottomDresses => Constants.LocalizationDressesPopupBottomDressTabTitleKey,
                    PlayerDressesPopupTabType.Hairs => Constants.LocalizationDressesPopupHairTabTitleKey,
                    PlayerDressesPopupTabType.Glasses => Constants.LocalizationDressesPopupGlassesTabTitleKey,
                    _ => string.Empty
                };
                _popupView.AddTab(_localizationProvider.GetLocale(tabTitleKey));
            }
        }
        
        private void SubscribeOnItemView(UIDressesPopupItemView itemView)
        {
            itemView.ButtonClicked += OnItemButtonClicked;
        }

        private void UnsubscribeFromItemView(UIDressesPopupItemView itemView)
        {
            itemView.ButtonClicked -= OnItemButtonClicked;
        }

        private void OnItemButtonClicked(UIDressesPopupItemView itemView)
        {
            _audioPlayer.PlaySound(SoundIdKey.Button_1);
            
            var itemViewModel = _viewModelByView[itemView];
            
            _eventBus.Dispatch(new UIDressesPopupItemClickedEvent(itemViewModel));
        }
        
        private void DressesFlagChanged()
        {
            UpdateNewNotificationsOnTabs();
        }
        
        private void UpdateNewNotificationsOnTabs()
        {
            var tabIndex = Array.IndexOf(TargetModel.TabTypes, PlayerDressesPopupTabType.TopDresses);
            _popupView.SetTabNewNotificationVisibility(tabIndex, _uiFlagsModel.HaveNewTopDresses);
            
            tabIndex = Array.IndexOf(TargetModel.TabTypes, PlayerDressesPopupTabType.BottomDresses);
            _popupView.SetTabNewNotificationVisibility(tabIndex, _uiFlagsModel.HaveNewBottomDresses);
            
            tabIndex = Array.IndexOf(TargetModel.TabTypes, PlayerDressesPopupTabType.Hairs);
            _popupView.SetTabNewNotificationVisibility(tabIndex, _uiFlagsModel.HaveNewHairs);
            
            tabIndex = Array.IndexOf(TargetModel.TabTypes, PlayerDressesPopupTabType.Glasses);
            _popupView.SetTabNewNotificationVisibility(tabIndex, _uiFlagsModel.HaveNewGlasses);
        }
        
        private void OnCloseButtonClicked()
        {
            DisappearAndRequestUnmediate().Forget();
        }
        
        private async UniTaskVoid DisappearAndRequestUnmediate()
        {
            _eventBus.Dispatch(new RequestGamePauseEvent(nameof(UIDressesPopupMediator), false));
            
            _audioPlayer.PlaySound(SoundIdKey.PopupClose);

            await _popupView.DisappearAsync();
            
            _eventBus.Dispatch(new UIRequestClosePopupEvent(TargetModel));
        }
        
        

        private static int GetMaxOfFourInts(int a, int b, int c, int d)
        {
            return Math.Max(Math.Max(Math.Max(a, b), c), d);
        }
    }
}