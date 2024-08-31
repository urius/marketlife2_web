using System.Collections.Generic;
using Holders;
using Infra.Instance;
using Model;
using Model.SpendPoints;

namespace View.Game.BuildPoint
{
    public class BuildPointsMediator : MediatorBase
    {
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly Dictionary<BuildPointModel, BuildPointMediator> _mediatorByModelDictionary = new();
        
        private ShopModel _shopModel;

        protected override void MediateInternal()
        {
            _shopModel = _shopModelHolder.ShopModel;
                
            MediateBuildPoints();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _shopModel.BuildPointAdded += OnBuildPointAdded;
            _shopModel.BuildPointRemoved += OnBuildPointRemoved;
        }

        private void Unsubscribe()
        {
            _shopModel.BuildPointAdded -= OnBuildPointAdded;
            _shopModel.BuildPointRemoved -= OnBuildPointRemoved;
        }

        private void OnBuildPointAdded(BuildPointModel buildPointModel)
        {
            MediateBuildPoint(buildPointModel);
        }

        private void OnBuildPointRemoved(BuildPointModel buildPointModel)
        {
            var mediator = _mediatorByModelDictionary[buildPointModel];
            UnmediateChild(mediator);
            _mediatorByModelDictionary.Remove(buildPointModel);
        }

        private void MediateBuildPoints()
        {
            var models = _shopModel.BuildPoints.Values;

            foreach (var buildPointModel in models)
            {
                MediateBuildPoint(buildPointModel);
            }
        }

        private void MediateBuildPoint(BuildPointModel buildPointModel)
        {
            var mediator = MediateChild<BuildPointMediator, BuildPointModel>(TargetTransform, buildPointModel);
            _mediatorByModelDictionary[buildPointModel] = mediator;
        }
    }
}