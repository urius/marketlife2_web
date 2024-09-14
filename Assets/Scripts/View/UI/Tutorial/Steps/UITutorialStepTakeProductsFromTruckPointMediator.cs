using System.IO;
using Cysharp.Threading.Tasks;
using Data;
using Holders;
using Infra.Instance;
using Model;
using UnityEngine;
using View.Helpers;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepTakeProductsFromTruckPointMediator : UITutorialStepMoveToMediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private ShopModel _shopModel;
        private PlayerCharModel _playerCharModel;
        private Vector2Int? _targetMoveToCell;

        protected override string MessageText =>
            _localizationProvider.GetLocale(Constants.LocalizationTutorialTakeProductsFromTruckPointMessageKey);

        protected override void MediateInternal()
        {
            base.MediateInternal();

            _shopModel = _playerModelHolder.PlayerModel.ShopModel;
            _playerCharModel = _playerModelHolder.PlayerCharModel;
        }

        protected override bool CheckStepConditions()
        {
            return CheckProductsDelivered();
        }

        protected override void ActivateStep()
        {
            if (_playerCharModel.HasProducts == false)
            {
                ActivateStepWithDelay().Forget();
            }
            else
            {
                DispatchStepFinished();
            }
        }

        private async UniTaskVoid ActivateStepWithDelay()
        {
            await UniTask.Delay(1000);
            
            base.ActivateStep();
        }

        protected override void Subscribe()
        {
            base.Subscribe();

            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
        }

        protected override void Unsubscribe()
        {
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            
            base.Unsubscribe();
        }

        private void OnGameplayFixedUpdate()
        {
            if (_playerCharModel.HasProducts)
            {
                _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
                
                DispatchStepFinished();
            }
        }

        protected override Vector2Int GetTargetMoveToCell()
        {
            if (_targetMoveToCell == null)
            {
                foreach (var truckPointModel in _shopModel.TruckPoints)
                {
                    if (truckPointModel.IsDelivered)
                    {
                        _targetMoveToCell = TruckPointHelper.GetClosestInteractionCell(truckPointModel.CellCoords,
                            _playerCharModel.CellPosition);
                        break;
                    }
                }
            }

            if (_targetMoveToCell == null)
            {
                throw new InvalidDataException($"{nameof(GetTargetMoveToCell)}: can't get truck point with delivered products");
            }

            return _targetMoveToCell.Value;
        }

        private bool CheckProductsDelivered()
        {
            foreach (var truckPointModel in _shopModel.TruckPoints)
            {
                if (truckPointModel.IsDelivered) return true;
            }

            return false;
        }
    }
}