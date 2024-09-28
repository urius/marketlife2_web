using System;
using Data;
using Events;
using Extensions;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Tools.AudioManager;
using UnityEngine;
using View.Game.Misc;
using Random = UnityEngine.Random;

namespace View.Game.People
{
    public class PlayerCharMoneyAnimationMediator : MediatorBase
    {
        private const int MoneySoundDelayFrames = 5;
        
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();

        private readonly TakeMoneyAnimationContext _takeMoneyAnimationContext = new();

        private int _moneySoundDelayCounter;
        private int _moneySoundsLeftCount = 0;

        protected override void MediateInternal()
        {
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            ClearTakeMoneyAnimation();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<AnimateTakeMoneyFromCashDeskEvent>(OnAnimateTakeMoneyFromCashDeskEvent);
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<AnimateTakeMoneyFromCashDeskEvent>(OnAnimateTakeMoneyFromCashDeskEvent);
            
            _updatesProvider.GameplayFixedUpdate -= OnAnimateTakeMoneyGameplayFixedUpdate;
        }

        private void OnAnimateTakeMoneyFromCashDeskEvent(AnimateTakeMoneyFromCashDeskEvent e)
        {
            var moneyPositionProvider = _sharedViewsDataHolder.GetCashDeskMoneyPositionProvider(e.TargetCashDesk);
            var playerCharPositionProvider = _sharedViewsDataHolder.GetPlayerCharPositionProvider();

            var itemsAmount = Math.Min(e.MoneyAmount, 30);
            var startPositions = new Vector3[itemsAmount];
            var moneyViews = new MoneyView[itemsAmount];
            var progresses = new float[itemsAmount];
            
            for (var i = 0; i < itemsAmount; i++)
            {
                var moneyView = GetFromCache<MoneyView>(PrefabKey.MoneyCashDesk);
                moneyView.SetSortingOrder(100);
                moneyView.SetAlpha(1);
                var pos = moneyPositionProvider.GetMoneySlotWorldPosition(i);
                moneyViews[i] = moneyView;
                startPositions[i] = pos;
                progresses[i] -= Random.Range(0, 1f);
            }

            ClearTakeMoneyAnimation();
            _takeMoneyAnimationContext.MoneyViews = moneyViews;
            _takeMoneyAnimationContext.StartPositions = startPositions;
            _takeMoneyAnimationContext.TargetTransform = playerCharPositionProvider.CenterPointTransform;
            _takeMoneyAnimationContext.Progresses = progresses;

            _updatesProvider.GameplayFixedUpdate -= OnAnimateTakeMoneyGameplayFixedUpdate;
            _updatesProvider.GameplayFixedUpdate += OnAnimateTakeMoneyGameplayFixedUpdate;

            _moneySoundsLeftCount = Math.Min(itemsAmount, 10);
        }

        private void OnAnimateTakeMoneyGameplayFixedUpdate()
        {
            if (_takeMoneyAnimationContext.IsFinished)
            {
                ClearTakeMoneyAnimation();
                _moneySoundsLeftCount = 0;
                return;
            }
            
            MoneyView tempMoneyView = null;
            var deltaProgress = Time.deltaTime * 5;
            _takeMoneyAnimationContext.IsFinished = true;
            
            for (var i = 0; i < _takeMoneyAnimationContext.MoneyViews.Length; i++)
            {
                _takeMoneyAnimationContext.Progresses[i] += deltaProgress;
                var progress = _takeMoneyAnimationContext.Progresses[i];
                progress = progress < 0 ? 0 : progress;

                tempMoneyView = _takeMoneyAnimationContext.MoneyViews[i];
                tempMoneyView.transform.position = Vector3.Lerp(
                    _takeMoneyAnimationContext.StartPositions[i],
                    _takeMoneyAnimationContext.TargetTransform.position,
                    progress);
                tempMoneyView.SetAlpha(1.1f - progress);

                if (progress < 1)
                {
                    _takeMoneyAnimationContext.IsFinished = false;
                }
            }

            ProcessMoneySoundPlay();
        }

        private void ProcessMoneySoundPlay()
        {
            if (_moneySoundsLeftCount > 0
                && _moneySoundDelayCounter <= 0)
            {
                _moneySoundDelayCounter = MoneySoundDelayFrames;
                _moneySoundsLeftCount--;
                
                _audioPlayer.PlaySound(SoundIdKey.TakeMoneyFromCashDesk);
            }
            else if (_moneySoundDelayCounter > 0)
            {
                _moneySoundDelayCounter--;
            }
        }

        private void ClearTakeMoneyAnimation()
        {
            _updatesProvider.GameplayFixedUpdate -= OnAnimateTakeMoneyGameplayFixedUpdate;

            if (_takeMoneyAnimationContext.MoneyViews != null)
            {
                foreach (var moneyView in _takeMoneyAnimationContext.MoneyViews)
                {
                    ReturnToCache(moneyView.gameObject);
                }
            }

            _takeMoneyAnimationContext.Reset();
        }
        
        private class TakeMoneyAnimationContext
        {
            public Vector3[] StartPositions;
            public MoneyView[] MoneyViews;
            public Transform TargetTransform;
            public float[] Progresses;
            public bool IsFinished;

            public void Reset()
            {
                StartPositions = null;
                MoneyViews = null;
                TargetTransform = null;
                Progresses = null;
                IsFinished = false;
            }
        }
    }
}