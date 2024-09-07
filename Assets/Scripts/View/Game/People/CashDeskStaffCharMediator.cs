using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.People;
using UnityEngine;
using Utils;
using View.Helpers;

namespace View.Game.People
{
    public class CashDeskStaffCharMediator : BotCharMediatorBase<CashDeskStaffModel>
    {
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        
        private ManClockView _clockView;
        private PlayerModel _playerModel;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            
            base.MediateInternal();

            ManView.transform.position = 0.5f *
                                         (_gridCalculator.GetCellCenterWorld(TargetModel.CellCoords + Vector2Int.right) +
                                          _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords));
                
            ManView.ToRightSide();

            SetClothes();
            
            _clockView = InstantiatePrefab<ManClockView>(PrefabKey.ManClockIcon);
            UpdateClockPosition();
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_clockView);
            _clockView = null;
            
            base.UnmediateInternal();
        }

        private void UpdateClockPosition()
        {
            _clockView.transform.position = ManView.transform.position;
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<RequestCashDeskStaffAcceptingPayAnimationEvent>(OnRequestCashDeskStaffAcceptingPayAnimationEvent);
            
            TargetModel.WorkSecondsLeftChanged += OnWorkSecondsLeftChanged;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<RequestCashDeskStaffAcceptingPayAnimationEvent>(OnRequestCashDeskStaffAcceptingPayAnimationEvent);

            TargetModel.WorkSecondsLeftChanged -= OnWorkSecondsLeftChanged;
        }

        private void OnRequestCashDeskStaffAcceptingPayAnimationEvent(RequestCashDeskStaffAcceptingPayAnimationEvent e)
        {
            if (e.StaffModel != TargetModel) return;

            if (e.RequestFinishAnimation == false)
            {
                ManView.ToProcessingPayState();
            }
            else
            {
                ManView.ToIdleState();
            }
        }

        private void OnWorkSecondsLeftChanged(int secondsLeft)
        {
            UpdateClockIconColor();
        }

        private void UpdateClockIconColor()
        {
            var color = StaffCharHelper.GetClockColorByPercent((float)TargetModel.WorkSecondsLeft / _playerModel.StaffWorkTimeSeconds);
            _clockView.SetIconColor(color);
        }

        private void SetClothes()
        {
            var clothes = ManSpriteTypesHelper.GetCashDeskStaffClothes();
            var hair = ManSpriteTypesHelper.GetRandomHair();
            
            SetBaseClothes(clothes.BodyClothes, clothes.HandClothes, clothes.FootClothes, hair);
            SetHat(ManSpriteType.None);

            SetRandomGlasses();
        }

        protected override void ToWalkingState()
        {
        }

        protected override void ToIdleState()
        {
        }

        protected override void StepFinishedHandler()
        {
        }
    }
}