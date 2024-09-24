using Cysharp.Threading.Tasks;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using UnityEngine;
using View.UI.Common;

namespace View.UI.TopPanel
{
    public class UITopPanelButtonsViewMediator : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IInteriorDataProvider _interiorDataProvider = Instance.Get<IInteriorDataProvider>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IPlayerFocusProvider _playerFocusProvider = Instance.Get<IPlayerFocusProvider>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private UITopPanelButtonsView _buttonsView;
        private PlayerModel _playerModel;
        private UISimpleButtonView _interiorButton;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            
            _buttonsView = TargetTransform.GetComponent<UITopPanelButtonsView>();
            _interiorButton = _buttonsView.InteriorButton;

            var shouldShowInteriorButton = ShouldShowInteriorButton();
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
            Unsubscribe();
            
            _buttonsView = null;
        }

        private void Subscribe()
        {
            _playerModel.LevelChanged += OnLevelChanged;
            
            _interiorButton.Clicked += OnInteriorButtonClicked;
        }

        private void Unsubscribe()
        {            
            _playerModel.LevelChanged -= OnLevelChanged;

            _interiorButton.Clicked -= OnInteriorButtonClicked;
        }

        private void OnLevelChanged(int level)
        {
            if (_buttonsView.InteriorButton.IsVisible == false
                && ShouldShowInteriorButton())
            {
                _updatesProvider.QuarterSecondPassed -= OnShowInteriorButtonQuarterSecondPassed;
                _updatesProvider.QuarterSecondPassed += OnShowInteriorButtonQuarterSecondPassed;
            }
        }

        private void OnShowInteriorButtonQuarterSecondPassed()
        {
            if (_playerFocusProvider.IsPlayerFocused)
            {
                _updatesProvider.QuarterSecondPassed -= OnShowInteriorButtonQuarterSecondPassed;

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

        private static void SetInteriorButtonShownSharedFlag(bool value)
        {
            SharedFlagsHolder.Instance.Set(SharedFlagKey.UITopPanelInteriorButtonShown, value);
        }

        private void OnInteriorButtonClicked()
        {
            _eventBus.Dispatch(new UIInteriorButtonClickedEvent());
        }
    }
}