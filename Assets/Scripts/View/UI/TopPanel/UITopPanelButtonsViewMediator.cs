using Cysharp.Threading.Tasks;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using UnityEngine;
using View.UI.SettingsCanvas;

namespace View.UI.TopPanel
{
    public class UITopPanelButtonsViewMediator : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IInteriorDataProvider _interiorDataProvider = Instance.Get<IInteriorDataProvider>();
        private readonly IPlayerDressesDataProvider _dressesDataProvider = Instance.Get<IPlayerDressesDataProvider>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IPlayerFocusProvider _playerFocusProvider = Instance.Get<IPlayerFocusProvider>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly ISharedFlagsHolder _sharedFlagsHolder = Instance.Get<ISharedFlagsHolder>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        
        private UITopPanelButtonsView _buttonsView;
        private PlayerModel _playerModel;
        private UITopPanelButtonView _interiorButton;
        private UITopPanelButtonView _dressesButton;
        private PlayerUIFlagsModel _uiFlagsModel;
        private UISettingsCanvasView _settingsCanvasView;
        private UITopPanelButtonView _leaderboardButton;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            _uiFlagsModel = _playerModel.UIFlagsModel;
            
            _buttonsView = TargetTransform.GetComponent<UITopPanelButtonsView>();
            _interiorButton = _buttonsView.InteriorButton;
            _dressesButton = _buttonsView.DressesButton;
            _leaderboardButton = _buttonsView.LeaderboardButton;
            _settingsCanvasView = _sharedViewsDataHolder.GetSettingsCanvasView();
            
            _sharedViewsDataHolder.RegisterTopPanelInteriorButtonTransform(_interiorButton.RectTransform);
            _sharedViewsDataHolder.RegisterTopPanelDressesButtonTransform(_dressesButton.RectTransform);
            _sharedViewsDataHolder.RegisterTopPanelLeaderboardButtonTransform(_leaderboardButton.RectTransform);

            var shouldShowInteriorButton = ShouldShowInteriorButton();
            UpdateInteriorButtonNewNotificationVisibility();
            _interiorButton.SetVisibility(shouldShowInteriorButton);
            SetInteriorButtonShownSharedFlag(shouldShowInteriorButton);
            
            var shouldShowDressesButton = ShouldShowDressesButton();
            UpdateDressesButtonNewNotificationVisibility();
            _dressesButton.SetVisibility(shouldShowDressesButton);
            SetDressesButtonShownSharedFlag(shouldShowDressesButton);
            
            var shouldShowLeaderboardButton = ShouldShowLeaderboardButton();
            _leaderboardButton.SetVisibility(shouldShowLeaderboardButton);
            SetLeaderboardButtonShownSharedFlag(shouldShowLeaderboardButton);
            
            Subscribe();
        }

        private bool ShouldShowInteriorButton()
        {
            return _interiorDataProvider.GetWallItemsByLevel(_playerModel.Level).Length > 1
                   || _interiorDataProvider.GetFloorItemsByLevel(_playerModel.Level).Length > 1;
        }
        
        private bool ShouldShowDressesButton()
        {
            return _dressesDataProvider.GetTopBodyItemsByLevel(_playerModel.Level).Length > 1
                   || _dressesDataProvider.GetBottomBodyItemsByLevel(_playerModel.Level).Length > 1
                   || _dressesDataProvider.GetHairItemsByLevel(_playerModel.Level).Length > 1
                   || _dressesDataProvider.GetGlassItemsByLevel(_playerModel.Level).Length > 1;
        }
        
        private bool ShouldShowLeaderboardButton()
        {
            return ShouldShowInteriorButton() && ShouldShowDressesButton()
                                              && _playerModel.Level >= Constants.MinLevelForLeaderboardButton;
        }

        protected override void UnmediateInternal()
        {
            _sharedViewsDataHolder.UnregisterTopPanelInteriorButtonTransform();
            _sharedViewsDataHolder.UnregisterTopPanelDressesButtonTransform();
            
            Unsubscribe();
            
            _buttonsView = null;
        }

        private void Subscribe()
        {
            _playerModel.LevelChanged += OnLevelChanged;
            _uiFlagsModel.FloorsFlagChanged += OnFloorsFlagChanged;
            _uiFlagsModel.WallsFlagChanged += OnWallsFlagChanged;
            _uiFlagsModel.DressesFlagChanged += OnDressesFlagChanged;
            _settingsCanvasView.SettingsButtonClicked += OnSettingsButtonClicked;
            
            _interiorButton.Clicked += OnInteriorButtonClicked;
            _dressesButton.Clicked += OnDressesButtonClicked;
            _leaderboardButton.Clicked += OnLeaderboardButtonClicked;
        }

        private void Unsubscribe()
        {            
            _playerModel.LevelChanged -= OnLevelChanged;
            _uiFlagsModel.FloorsFlagChanged -= OnFloorsFlagChanged;
            _uiFlagsModel.WallsFlagChanged -= OnWallsFlagChanged;
            _uiFlagsModel.DressesFlagChanged -= OnDressesFlagChanged;
            _settingsCanvasView.SettingsButtonClicked -= OnSettingsButtonClicked;

            _interiorButton.Clicked -= OnInteriorButtonClicked;
            _dressesButton.Clicked -= OnDressesButtonClicked;
            _leaderboardButton.Clicked -= OnLeaderboardButtonClicked;
        }

        private void OnSettingsButtonClicked()
        {
            _eventBus.Dispatch(new UISettingsButtonClickedEvent());
        }

        private void OnWallsFlagChanged(bool flagValue)
        {
            UpdateInteriorButtonNewNotificationVisibility();
        }

        private void OnFloorsFlagChanged(bool flagValue)
        {
            UpdateInteriorButtonNewNotificationVisibility();
        }

        private void UpdateInteriorButtonNewNotificationVisibility()
        {
            var isNewNotificationVisible = _uiFlagsModel.HaveNewFloors || _uiFlagsModel.HaveNewWalls;
            _interiorButton.SetNewNotificationVisibility(isNewNotificationVisible);
        }

        private void OnDressesFlagChanged()
        {
            UpdateDressesButtonNewNotificationVisibility();
        }

        private void UpdateDressesButtonNewNotificationVisibility()
        {
            var isNewNotificationVisible = _uiFlagsModel.HaveNewTopDresses ||
                                           _uiFlagsModel.HaveNewBottomDresses ||
                                           _uiFlagsModel.HaveNewHairs ||
                                           _uiFlagsModel.HaveNewGlasses;
            
            _dressesButton.SetNewNotificationVisibility(isNewNotificationVisible);
        }

        private void OnLevelChanged(int level)
        {
            if (_buttonsView.InteriorButton.IsVisible == false
                && ShouldShowInteriorButton())
            {
                _updatesProvider.GameplayQuarterSecondPassed -= OnShowInteriorButtonQuarterSecondPassed;
                _updatesProvider.GameplayQuarterSecondPassed += OnShowInteriorButtonQuarterSecondPassed;
            }
            
            if (_buttonsView.DressesButton.IsVisible == false
                && ShouldShowDressesButton())
            {
                _updatesProvider.GameplayQuarterSecondPassed -= OnShowDressesButtonQuarterSecondPassed;
                _updatesProvider.GameplayQuarterSecondPassed += OnShowDressesButtonQuarterSecondPassed;
            }
            
            if (_buttonsView.LeaderboardButton.IsVisible == false
                && ShouldShowLeaderboardButton())
            {
                _updatesProvider.GameplayQuarterSecondPassed -= OnShowLeaderboardButtonQuarterSecondPassed;
                _updatesProvider.GameplayQuarterSecondPassed += OnShowLeaderboardButtonQuarterSecondPassed;
            }
        }

        private void OnShowInteriorButtonQuarterSecondPassed()
        {
            if (_playerFocusProvider.IsPlayerFocused)
            {
                _updatesProvider.GameplayQuarterSecondPassed -= OnShowInteriorButtonQuarterSecondPassed;

                _interiorButton.SetVisibility(isVisible: true);

                AnimateAppearing(_interiorButton.RectTransform)
                    .ContinueWith(() => SetInteriorButtonShownSharedFlag(true));
            }
        }
        
        private void OnShowDressesButtonQuarterSecondPassed()
        {
            if (_playerFocusProvider.IsPlayerFocused)
            {
                _updatesProvider.GameplayQuarterSecondPassed -= OnShowDressesButtonQuarterSecondPassed;

                _dressesButton.SetVisibility(isVisible: true);

                AnimateAppearing(_dressesButton.RectTransform)
                    .ContinueWith(() => SetDressesButtonShownSharedFlag(true));
            }
        }
        
        private void OnShowLeaderboardButtonQuarterSecondPassed()
        {
            if (_playerFocusProvider.IsPlayerFocused)
            {
                _updatesProvider.GameplayQuarterSecondPassed -= OnShowLeaderboardButtonQuarterSecondPassed;

                _leaderboardButton.SetVisibility(isVisible: true);

                AnimateAppearing(_leaderboardButton.RectTransform)
                    .ContinueWith(() => SetLeaderboardButtonShownSharedFlag(true));
            }
        }

        private UniTask AnimateAppearing(RectTransform rectTransform)
        {
            var currentPos = rectTransform.anchoredPosition;
            var targetY = currentPos.y;

            rectTransform.anchoredPosition = new Vector2(currentPos.x, targetY + 50);
            var (moveTask, descr) = LeanTweenHelper.MoveYAsync(rectTransform, targetY, 0.8f);
            
            descr.setEaseOutBounce();

            return moveTask;
        }

        private void SetInteriorButtonShownSharedFlag(bool value)
        {
            _sharedFlagsHolder.Set(SharedFlagKey.UITopPanelInteriorButtonShown, value);
        }

        private void SetDressesButtonShownSharedFlag(bool value)
        {
            _sharedFlagsHolder.Set(SharedFlagKey.UITopPanelDressesButtonShown, value);
        }

        private void SetLeaderboardButtonShownSharedFlag(bool value)
        {
            _sharedFlagsHolder.Set(SharedFlagKey.UITopPanelLeaderboardButtonShown, value);
        }

        private void OnInteriorButtonClicked()
        {
            _eventBus.Dispatch(new UIInteriorButtonClickedEvent());
        }

        private void OnDressesButtonClicked()
        {
            _eventBus.Dispatch(new UIDressesButtonClickedEvent());
        }

        private void OnLeaderboardButtonClicked()
        {
            _eventBus.Dispatch(new UILeaderboardButtonClickedEvent());
        }
    }
}