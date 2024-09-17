using System.IO;
using Cysharp.Threading.Tasks;
using Data;
using Holders;
using Infra.Instance;
using Model;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepPutProductsOnShelfMediator : UITutorialStepMoveToMediatorBase
    {
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        
        private PlayerCharModel _playerCharModel;
        private ShopModel _shopModel;

        protected override string MessageText =>
            _localizationProvider.GetLocale(Constants.LocalizationTutorialPutProductsOnShelfMessageKey);

        protected override void MediateInternal()
        {
            _playerCharModel = _playerModelHolder.PlayerCharModel;
            _shopModel = _playerModelHolder.PlayerModel.ShopModel;
            
            base.MediateInternal();
        }

        protected override bool CheckStepConditions()
        {
            return _playerCharModel.HasProducts;
        }

        protected override void ActivateStep()
        {
            ActivateStepWithDelay().Forget();
        }

        private async UniTaskVoid ActivateStepWithDelay()
        {
            await UniTask.Delay(500);
            
            base.ActivateStep();
        }

        protected override void Subscribe()
        {
            base.Subscribe();

            _playerCharModel.ProductRemoved += OnProductRemoved;
        }

        protected override void Unsubscribe()
        {
            _playerCharModel.ProductRemoved -= OnProductRemoved;
            
            base.Unsubscribe();
        }

        private void OnProductRemoved(int _)
        {
            if (_playerCharModel.HasProducts == false)
            {
                DispatchStepFinished();
            }
        }

        protected override Vector2Int GetTargetMoveToCell()
        {
            foreach (var shelf in _shopModel.Shelfs)
            {
                if (shelf.HasEmptySlots())
                {
                    return shelf.CellCoords;
                }
            }

            throw new InvalidDataException($"{nameof(GetTargetMoveToCell)}: failed to find shelf with empty slots");
        }
    }
}