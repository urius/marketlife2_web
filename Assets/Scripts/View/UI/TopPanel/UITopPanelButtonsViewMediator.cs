using Cysharp.Threading.Tasks;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using UnityEngine;

namespace View.UI.TopPanel
{
    public class UITopPanelButtonsViewMediator : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IInteriorDataProvider _interiorDataProvider = Instance.Get<IInteriorDataProvider>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IPlayerFocusProvider _playerFocusProvider = Instance.Get<IPlayerFocusProvider>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly ISharedFlagsHolder _sharedFlagsHolder = Instance.Get<ISharedFlagsHolder>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        
        private UITopPanelButtonsView _buttonsView;
        private PlayerModel _playerModel;
        private UITopPanelButtonView _interiorButton;
        private PlayerUIFlagsModel _uiFlagsModel;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            _uiFlagsModel = _playerModel.UIFlagsModel;
            
            _buttonsView = TargetTransform.GetComponent<UITopPanelButtonsView>();
            _interiorButton = _buttonsView.InteriorButton;
            
            _sharedViewsDataHolder.RegisterTopPanelInteriorButtonTransform(_interiorButton.RectTransform);

            var shouldShowInteriorButton = ShouldShowInteriorButton();
            UpdateInteriorButtonNewNotificationVisibility();
            _interiorButton.SetVisibility(shouldShowInteriorButton);
            SetInteriorButtonShownSharedFlag(shouldShowInteriorButton);
            
            Subscribe();
        }

        private bool ShouldShowInteriorButton()
        {
            return _interiorDataProvider.GetWallItemsByLevel(_playerModel.Level).Length > 1
                   || _interiorDataProvider.GetFloorItemsByLevel(_playerModel.Level).Length > 1;
        }

        protected override void UnmediateInternal()
        {
            _sharedViewsDataHolder.UnregisterTopPanelInteriorButtonTransform();
            
            Unsubscribe();
            
            _buttonsView = null;
        }

        private void Subscribe()
        {
            _playerModel.LevelChanged += OnLevelChanged;
            _uiFlagsModel.FloorsFlagChanged += OnFloorsFlagChanged;
            _uiFlagsModel.WallsFlagChanged += OnWallsFlagChanged;
            
            _interiorButton.Clicked += OnInteriorButtonClicked;
        }

        private void Unsubscribe()
        {            
            _playerModel.LevelChanged -= OnLevelChanged;
            _uiFlagsModel.FloorsFlagChanged -= OnFloorsFlagChanged;
            _uiFlagsModel.WallsFlagChanged -= OnWallsFlagChanged;

            _interiorButton.Clicked -= OnInteriorButtonClicked;
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

        private void OnLevelChanged(int level)
        {
            if (_buttonsView.InteriorButton.IsVisible == false
                && ShouldShowInteriorButton())
            {
                _updatesProvider.GameplayQuarterSecondPassed -= OnShowInteriorButtonQuarterSecondPassed;
                _updatesProvider.GameplayQuarterSecondPassed += OnShowInteriorButtonQuarterSecondPassed;
            }
        }

        private void OnShowInteriorButtonQuarterSecondPassed()
        {
            if (_playerFocusProvider.IsPlayerFocused)
            {
                _updatesProvider.GameplayQuarterSecondPassed -= OnShowInteriorButtonQuarterSecondPassed;

                _interiorButton.SetVisibility(isVisible: true);
                
                AnimateAppearing().Forget();
            }
        }

        private async UniTaskVoid AnimateAppearing()
        {
            var currentPos = _interiorButton.RectTransform.anchoredPosition;
            var targetY = currentPos.y;

            _interiorButton.RectTransform.anchoredPosition = new Vector2(currentPos.x, targetY + 50);
            var (moveTask, descr) = LeanTweenHelper.MoveYAsync(_interiorButton.RectTransform, targetY, 0.8f);
            
            descr.setEaseOutBounce();

            await moveTask;

            SetInteriorButtonShownSharedFlag(true);
        }

        private void SetInteriorButtonShownSharedFlag(bool value)
        {
            _sharedFlagsHolder.Set(SharedFlagKey.UITopPanelInteriorButtonShown, value);
        }

        private void OnInteriorButtonClicked()
        {
            _eventBus.Dispatch(new UIInteriorButtonClickedEvent());
        }
    }
}