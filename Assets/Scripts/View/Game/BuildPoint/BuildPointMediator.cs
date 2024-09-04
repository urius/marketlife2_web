using System.Collections.Generic;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.SpendPoints;
using UnityEngine;
using Utils;
using View.Game.Shared;

namespace View.Game.BuildPoint
{
    public class BuildPointMediator : MediatorWithModelBase<BuildPointModel>
    {
        private const float AnimDuration = 0.22f;
        private const float AnimDurationHalf = AnimDuration * 0.5f;

        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IPlayerCharViewSharedDataHolder _playerCharViewSharedDataHolder = Instance.Get<IPlayerCharViewSharedDataHolder>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();

        private readonly Queue<SpendAnimationContext> _contextsQueue = new();
        
        private BuildPointView _view;
        private PlayerModel _playerModel;

        private bool IsSpendLocked => TargetModel.BuildPointType == BuildPointType.Expand &&
                                      ExpandShopHelper.IsExpandUnlocked(TargetModel) == false;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            
            _view = InstantiatePrefab<BuildPointView>(PrefabKey.BuildPoint);
            
            _view.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);
            DisplayTooltip();

            DisplayFloorIcon();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();

            LeanTween.cancel(_view.gameObject);
            foreach (var spendAnimationContext in _contextsQueue)
            {
                LeanTween.cancel(spendAnimationContext.TargetMoneyGo);
                ReturnToCache(spendAnimationContext.TargetMoneyGo);
            }

            _contextsQueue.Clear();
            
            Destroy(_view);
            _view = null;
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<TriggerSpendMoneyOnBuildPointAnimationEvent>(OnTriggerSpendMoneyOnBuildPointAnimation);
            _eventBus.Subscribe<ExpandPointShownEvent>(OnExpandPointShownEvent);
            
            TargetModel.MoneyToBuildLeftChanged += OnMoneyToBuildLeftChanged;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<TriggerSpendMoneyOnBuildPointAnimationEvent>(OnTriggerSpendMoneyOnBuildPointAnimation);
            _eventBus.Unsubscribe<ExpandPointShownEvent>(OnExpandPointShownEvent);
            
            TargetModel.MoneyToBuildLeftChanged -= OnMoneyToBuildLeftChanged;
        }

        private void OnExpandPointShownEvent(ExpandPointShownEvent e)
        {
            if (e.CellPosition != TargetModel.CellCoords) return;
            
            DisplayTooltip();
        }

        private void DisplayTooltip()
        {
            DisplayTooltipIcon();
            DisplayTooltipText();
        }

        private void DisplayTooltipText()
        {
            if (IsSpendLocked)
            {
                var expandLevel = ExpandShopHelper.GetExpandLevelByExpandPoint(TargetModel);
                var levelText = $"{_localizationProvider.GetLocale(Constants.LocalizationKeyLevel)} {expandLevel}";
                _view.SetText(levelText);
            }
            else
            {
                DisplayMoneyText(TargetModel.MoneyToBuildLeft);
            }
        }

        private void DisplayTooltipIcon()
        {
            if (IsSpendLocked)
            {
                var starSprite = _spritesHolderSo.GetCommonSpriteByKey(SpriteKey.GUIStar);
                _view.SetIconOnTooltipSprite(starSprite);
            }
            else
            {
                var moneySprite = _spritesHolderSo.GetCommonSpriteByKey(SpriteKey.GUIMoney50);
                _view.SetIconOnTooltipSprite(moneySprite);
            }
        }
        

        private void DisplayFloorIcon()
        {
            if (TargetModel.BuildPointType == BuildPointType.Expand)
            {
                var isExpandX = ExpandShopHelper.IsExpandX(TargetModel.CellCoords);
                var sprite = _spritesHolderSo.GetCommonSpriteByKey(isExpandX ? SpriteKey.ExpandX : SpriteKey.ExpandY);
                _view.SetIconOnSquareSprite(sprite);
            }
        }

        private void OnTriggerSpendMoneyOnBuildPointAnimation(TriggerSpendMoneyOnBuildPointAnimationEvent e)
        {
            if (e.BuildPoint != TargetModel) return;
            
            var moneyGo = GetFromCache(PrefabKey.Money);
            var targetCellCoords = e.BuildPoint.CellCoords;
            
            var context = new SpendAnimationContext(moneyGo, targetCellCoords, e.FinishMoneyAmount);

            DisplayMoneyText(e.StartMoneyAmount);
            
            _contextsQueue.Enqueue(context);

            var pos = _playerCharViewSharedDataHolder.PlayerCharPosition;
            pos.z = Random.value * 0.3f + 0.3f;
            moneyGo.transform.position = pos;
            moneyGo.transform.Rotate(Vector3.forward, Random.value * 360);

            var targetPoint = _gridCalculator.GetCellCenterWorld(targetCellCoords);

            LeanTween.delayedCall(_view.gameObject, AnimDurationHalf, OnSpendAnimationHalf);
            LeanTween.move(moneyGo, targetPoint, AnimDuration)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(OnSpendAnimationComplete);
        }

        private void OnSpendAnimationHalf()
        {
            var context = _contextsQueue.Peek();
            _eventBus.Dispatch(new SpendMoneyOnBuildPointAnimationHalfEvent(context.TargetCellCoords, _contextsQueue.Count));
        }

        private void OnSpendAnimationComplete()
        {
            var context = _contextsQueue.Dequeue();
            
            ReturnToCache(context.TargetMoneyGo);
            
            DisplayMoneyText(context.MoneyToBuildLeftOnFinish);

            if (_contextsQueue.Count <= 0)
            {
                _eventBus.Dispatch(new SpendMoneyOnBuildPointLastAnimationFinishedEvent(context.TargetCellCoords));
            }
        }

        private void OnMoneyToBuildLeftChanged()
        {
            DisplayMoneyText(TargetModel.MoneyToBuildLeft);
        }

        private void DisplayMoneyText(int moneyAmount)
        {
            _view.SetText(moneyAmount.ToString());
        }
        
        private readonly struct SpendAnimationContext
        {
            public readonly GameObject TargetMoneyGo;
            public readonly Vector2Int TargetCellCoords;
            public readonly int MoneyToBuildLeftOnFinish;

            public SpendAnimationContext(GameObject targetMoneyGo, Vector2Int targetCellCoords,
                int moneyToBuildLeftOnFinish)
            {
                TargetMoneyGo = targetMoneyGo;
                TargetCellCoords = targetCellCoords;
                MoneyToBuildLeftOnFinish = moneyToBuildLeftOnFinish;
            }
        }
    }
}