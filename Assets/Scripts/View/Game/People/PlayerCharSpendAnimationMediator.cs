using System.Collections.Generic;
using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;
using Utils;

namespace View.Game.People
{
    public class PlayerCharSpendAnimationMediator : MediatorBase
    {
        private const float AnimDuration = 0.2f;
        private const float AnimDurationHalf = AnimDuration * 0.5f;
        
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();

        private readonly Queue<SpendAnimationContext> _contextsQueue = new();
        
        private ManView _playerCharView;

        public PlayerCharSpendAnimationMediator(ManView playerCharView)
        {
            _playerCharView = playerCharView;
        }

        protected override void MediateInternal()
        {
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_playerCharView);
            _playerCharView = null;
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<TriggerSpendMoneyOnBuildPointAnimationEvent>(OnTriggerSpendMoneyOnBuildPointAnimation);
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<TriggerSpendMoneyOnBuildPointAnimationEvent>(OnTriggerSpendMoneyOnBuildPointAnimation);
        }

        private void OnTriggerSpendMoneyOnBuildPointAnimation(TriggerSpendMoneyOnBuildPointAnimationEvent e)
        {
            var moneyGo = GetFromCache(PrefabKey.Money);
            var targetCellCoords = e.BuildPoint.CellCoords;
            
            var context = new SpendAnimationContext(moneyGo, targetCellCoords);
            
            _contextsQueue.Enqueue(context);
            
            var pos = _playerCharView.transform.position;
            pos.z = Random.value * 0.3f + 0.3f;
            moneyGo.transform.position = pos;
            moneyGo.transform.Rotate(Vector3.forward, Random.value * 360);

            var targetPoint = _gridCalculator.GetCellCenterWorld(targetCellCoords);

            LeanTween.delayedCall(AnimDurationHalf, OnSpendAnimationHalf);
            LeanTween.move(moneyGo, targetPoint, AnimDuration).setOnComplete(OnSpendAnimationComplete);
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
            
            _eventBus.Dispatch(new SpendMoneyOnBuildPointAnimationFinishedEvent(context.TargetCellCoords));
        }
        
        private struct SpendAnimationContext
        {
            public readonly GameObject TargetMoneyGo;
            public readonly Vector2Int TargetCellCoords;

            public SpendAnimationContext(GameObject targetMoneyGo, Vector2Int targetCellCoords)
            {
                TargetMoneyGo = targetMoneyGo;
                TargetCellCoords = targetCellCoords;
            }
        }
    }
}