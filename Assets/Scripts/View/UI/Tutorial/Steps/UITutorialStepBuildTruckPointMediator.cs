using System;
using Data;
using Holders;
using Infra.Instance;
using Model;
using Model.SpendPoints;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepBuildTruckPointMediator : UITutorialStepMoveToMediatorBase
    {
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();

        private UITutorialTextStepView _stepView;
        private ShopModel _shopModel;

        protected override void MediateInternal()
        {
            _shopModel = _shopModelHolder.ShopModel;
            
            base.MediateInternal();
        }

        protected override bool CheckStepConditions()
        {
            return TryGetTargetBuildPoint(out _);
        }

        protected override string MessageText =>
            _localizationProvider.GetLocale(Constants.LocalizationTutorialBuildTruckPointMessageKey);

        protected override Vector2Int GetTargetMoveToCell()
        {
            if (TryGetTargetBuildPoint(out var targetBuildPoint))
            {
                return targetBuildPoint.CellCoords;
            }

            throw new InvalidOperationException($"{nameof(GetTargetMoveToCell)}: Failed to get build point model");
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            
            _shopModel.BuildPointRemoved += OnBuildPointRemoved;
        }

        protected override void Unsubscribe()
        {
            _shopModel.BuildPointRemoved -= OnBuildPointRemoved;
            
            base.Unsubscribe();
        }

        private bool TryGetTargetBuildPoint(out BuildPointModel result)
        {
            result = null;
            
            foreach (var buildPoint in _shopModel.BuildPoints.Values)
            {
                if (buildPoint.ShopObjectType == ShopObjectType.TruckPoint)
                {
                    result = buildPoint;
                    break;
                }
            }

            return result != null;
        }

        private void OnBuildPointRemoved(BuildPointModel buildPointModel)
        {
            if (buildPointModel.CellCoords == TargetMoveToCell)
            {
                DispatchStepFinished();
            }
        }
    }
}