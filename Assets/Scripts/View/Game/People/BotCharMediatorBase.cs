using Data;
using Holders;
using Infra.Instance;
using Model.People;
using UnityEngine;
using Utils;
using View.Game.Shared;
using View.Helpers;

namespace View.Game.People
{
    public abstract class BotCharMediatorBase<TModel> : MediatorWithModelBase<TModel> 
        where TModel : BotCharModelBase
    {
        private const int Speed = 2;
        
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();

        private readonly WalkContext _walkContext = new ();

        protected ManView ManView { get; private set; }

        protected abstract void ToWalkingState();
        protected abstract void ToIdleState();
        protected abstract void StepFinishedHandler();
        
        protected override void MediateInternal()
        {
            ManView = InstantiatePrefab<ManView>(PrefabKey.Man);
            ManView.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);
            
            UpdateSorting(TargetModel.CellCoords);
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(ManView);
            ManView = null;
        }

        protected void DisableSwitchToIdleOnNextFrame()
        {
            _updatesProvider.GameplayFixedUpdate -= GameplayFixedUpdateWalkHandler;
            _walkContext.IdleStateRequestFlag = false;
        }

        protected void SetBaseClothes(
            ManSpriteType bodyClothes,
            ManSpriteType handClothes,
            ManSpriteType footClothes,
            ManSpriteType hairType)
        {
            var bodySprite = _spritesHolderSo.GetManSpriteByKey(bodyClothes);
            var handSprite = _spritesHolderSo.GetManSpriteByKey(handClothes);
            var footSprite = _spritesHolderSo.GetManSpriteByKey(footClothes);
            var hairSprite = _spritesHolderSo.GetManSpriteByKey(hairType);

            ManView.SetClothesSprites(bodySprite, handSprite, footSprite);
            ManView.SetHairSprite(hairSprite);
        }

        protected void SetGlasses(ManSpriteType glassesType)
        {
            if (glassesType == ManSpriteType.None)
            {
                ManView.SetGlassesSprite(null);
            }
            else
            {
                var glassesSprite = _spritesHolderSo.GetManSpriteByKey(glassesType);
                ManView.SetGlassesSprite(glassesSprite);
            }
        }

        protected void SetHat(ManSpriteType hatType)
        {
            if (hatType == ManSpriteType.None)
            {
                ManView.SetHatSprite(null);
            }
            else
            {
                var hatSprite = _spritesHolderSo.GetManSpriteByKey(ManSpriteType.SantaHat);
                ManView.SetHatSprite(hatSprite);
            }
        }
        
        protected void SetRandomGlasses()
        {
            SetGlasses(Random.value < 0.8 ? ManSpriteType.None : ManSpriteTypesHelper.GetRandomGlasses());
        }

        protected virtual void ProcessWalk()
        {
            ManView.transform.position = Vector3.Lerp(
                _walkContext.StartWalkPosition, _walkContext.EndWalkPosition, _walkContext.Progress);
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
                ManView.ToRightSide();
            }
            else if (deltaX < 0 || deltaY > 0)
            {
                ManView.ToLeftSide();
            }

            _walkContext.StartWalkPosition = ManView.transform.position;
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

            ProcessWalk();

            if (_walkContext.SteppedToNewCellFlag == false 
                && _walkContext.Progress > 0.5f
                && _gridCalculator.WorldToCell(ManView.transform.position) == TargetModel.CellCoords)
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
            DynamicViewSortingLogic.UpdateSorting(ManView, _ownedCellsDataHolder, cellCoords);
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