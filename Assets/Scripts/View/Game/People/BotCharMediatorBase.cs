using Holders;
using Infra.Instance;
using Model.People;
using UnityEngine;
using Utils;
using View.Game.Shared;

namespace View.Game.People
{
    public abstract class BotCharMediatorBase<TModel> : MediatorWithModelBase<TModel> 
        where TModel : ShopCharModelBase
    {
        private const int Speed = 2;
        
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();

        private readonly WalkContext _walkContext = new ();

        protected abstract ManView View { get; }
        
        protected abstract void ToWalkingState();
        protected abstract void ToIdleState();
        protected abstract void StepFinishedHandler();
        
        protected override void MediateInternal()
        {
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        protected void DisableSwitchToIdleOnNextFrame()
        {
            _updatesProvider.GameplayFixedUpdate -= GameplayFixedUpdateWalkHandler;
            _walkContext.IdleStateRequestFlag = false;
        }

        private void Subscribe()
        {
            TargetModel.CellPositionChanged += OnCellPositionChanged;
        }

        private void Unsubscribe()
        {
            TargetModel.CellPositionChanged -= OnCellPositionChanged;

            _updatesProvider.GameplayFixedUpdate -= GameplayFixedUpdateWalkHandler;
        }

        private void OnCellPositionChanged(Vector2Int cellCoords)
        {
            _walkContext.IdleStateRequestFlag = false;

            ToWalkingState();

            var deltaX = cellCoords.x - TargetModel.PreviousCellPosition.x;
            var deltaY = cellCoords.y - TargetModel.PreviousCellPosition.y;
            
            if (deltaX > 0 || deltaY < 0)
            {
                View.ToRightSide();
            }
            else if (deltaX < 0 || deltaY > 0)
            {
                View.ToLeftSide();
            }

            _walkContext.StartWalkPosition = View.transform.position;
            _walkContext.EndWalkPosition = _gridCalculator.GetCellCenterWorld(cellCoords);
            _walkContext.Progress = 0;
            _walkContext.SteppedToNewCellFlag = false;

            _updatesProvider.GameplayFixedUpdate -= GameplayFixedUpdateWalkHandler;
            _updatesProvider.GameplayFixedUpdate += GameplayFixedUpdateWalkHandler;
        }

        private void GameplayFixedUpdateWalkHandler()
        {
            if (_walkContext.IdleStateRequestFlag)
            {
                _walkContext.IdleStateRequestFlag = false;
                
                _updatesProvider.GameplayFixedUpdate -= GameplayFixedUpdateWalkHandler;
                
                ToIdleState();
                
                return;
            }
            
            _walkContext.Progress += Time.fixedDeltaTime * Speed;

            View.transform.position = Vector3.Lerp(
                _walkContext.StartWalkPosition, _walkContext.EndWalkPosition, _walkContext.Progress);

            if (_walkContext.SteppedToNewCellFlag == false 
                && _walkContext.Progress > 0.5f
                && _gridCalculator.WorldToCell(View.transform.position) == TargetModel.CellCoords)
            {
                _walkContext.SteppedToNewCellFlag = true;

                OnSteppedToNewCell();
            }

            if (_walkContext.Progress >= 1)
            {
                _walkContext.IdleStateRequestFlag = true;
                
                StepFinishedHandler();
            }
        }

        private void OnSteppedToNewCell()
        {
            UpdateSorting(TargetModel.CellCoords);
        }

        private void UpdateSorting(Vector2Int cellCoords)
        {
            DynamicViewSortingLogic.UpdateSorting(View, _ownedCellsDataHolder, cellCoords);
        }

        private class WalkContext
        {
            public Vector3 StartWalkPosition;
            public Vector3 EndWalkPosition;
            public float Progress;
            public bool SteppedToNewCellFlag;
            public bool IdleStateRequestFlag;
        }
    }
}