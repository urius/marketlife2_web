using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public abstract class UITutorialStepMoveToMediatorBase : UITutorialStepMediatorBase
    {
        private readonly IPlayerFocusProvider _playerFocusProvider = Instance.Get<IPlayerFocusProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();

        protected Vector2Int TargetMoveToCell;
        
        private Vector2 _targetMoveToWorldPosition;
        private UITutorialTextStepView _stepView;

        protected abstract string MessageText { get; }

        protected override void MediateInternal()
        {
        }

        protected override void ActivateStep()
        {
            TargetMoveToCell = GetTargetMoveToCell();

            _eventBus.Dispatch(new RequestCompassEvent(TargetMoveToCell));
            
            _stepView = InstantiateColdPrefab<UITutorialTextStepView>(Constants.TutorialDefaultStepWithTextPath);
            
            _stepView.SetText(MessageText);
            
            Subscribe();

            UpdateVisibility();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            _eventBus.Dispatch(new RequestRemoveCompassEvent(TargetMoveToCell));
            
            if (_stepView != null)
            {
                Destroy(_stepView);
                _stepView = null;
            }
        }

        protected virtual void Subscribe()
        {
            _playerFocusProvider.PlayerFocusChanged += OnPlayerFocusChanged;
        }

        protected virtual void Unsubscribe()
        {
            _playerFocusProvider.PlayerFocusChanged -= OnPlayerFocusChanged;
        }

        protected abstract Vector2Int GetTargetMoveToCell();

        // private void UpdateArrowRotation()
        // {
        //     var playerPosition = (Vector2)_playerCharPositionsProvider.RootTransform.position;
        //
        //     var angle = Vector2.SignedAngle(_targetMoveToWorldPosition - playerPosition, new Vector2(0, -1));
        //
        //     _stepView.SetArrowAngle(angle);
        // }

        private void UpdateVisibility()
        {
            _stepView.SetVisibility(_playerFocusProvider.IsPlayerFocused);
        }

        private void OnPlayerFocusChanged(bool isFocused)
        {
            UpdateVisibility();
        }
    }
}