using Data;
using Holders;
using Infra.Instance;
using Model;
using UnityEngine;
using Utils;
using View.Game.People;

namespace View.UI.Tutorial.Steps
{
    public abstract class UITutorialStepMoveToMediatorBase : UITutorialStepMediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IPlayerFocusProvider _playerFocusProvider = Instance.Get<IPlayerFocusProvider>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();

        protected Vector2Int TargetMoveToCell;
        
        private Vector2 _targetMoveToWorldPosition;
        private UITutorialTextStepView _stepView;
        private PlayerCharModel _playerCharModel;
        private IPlayerCharPositionsProvider _playerCharPositionsProvider;

        protected abstract string MessageText { get; }

        protected override void MediateInternal()
        {
            _playerCharModel = _playerModelHolder.PlayerCharModel;
            _playerCharPositionsProvider = _sharedViewsDataHolder.GetPlayerCharPositionProvider();
        }

        protected override void ActivateStep()
        {
            TargetMoveToCell = GetTargetMoveToCell();
            
            _targetMoveToWorldPosition = _gridCalculator.GetCellCenterWorld(TargetMoveToCell);
            
            _stepView = InstantiateColdPrefab<UITutorialTextStepView>(Constants.TutorialDefaultStepWithTextPath);
            
            _stepView.SetText(MessageText);
            UpdateArrowRotation();
            
            Subscribe();

            UpdateVisibility();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            if (_stepView != null)
            {
                Destroy(_stepView);
                _stepView = null;
            }
        }

        protected virtual void Subscribe()
        {
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
            _playerFocusProvider.PlayerFocusChanged += OnPlayerFocusChanged;
            _playerCharModel.CellPositionChanged += OnCellPositionChanged;
        }

        protected virtual void Unsubscribe()
        {
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            _playerFocusProvider.PlayerFocusChanged -= OnPlayerFocusChanged;
            _playerCharModel.CellPositionChanged -= OnCellPositionChanged;
        }

        protected abstract Vector2Int GetTargetMoveToCell();

        private void OnGameplayFixedUpdate()
        {
            UpdateArrowRotation();
        }

        private void UpdateArrowRotation()
        {
            var playerPosition = (Vector2)_playerCharPositionsProvider.RootTransform.position;

            var angle = Vector2.SignedAngle(_targetMoveToWorldPosition - playerPosition, new Vector2(0, -1));

            _stepView.SetArrowAngle(angle);
        }

        private void OnCellPositionChanged(Vector2Int newCellPosition)
        {
            _stepView.SetArrowVisibility(newCellPosition != TargetMoveToCell);
        }

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