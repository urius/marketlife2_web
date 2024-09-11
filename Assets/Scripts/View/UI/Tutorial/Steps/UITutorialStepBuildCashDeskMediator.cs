using System.Linq;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using UnityEngine;
using Utils;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepBuildCashDeskMediator : UITutorialStepMediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();

        private UITutorialTextStepView _stepView;
        private Vector2 _targetPosition;
        private ShopModel _shopModel;

        protected override bool CheckStepConditions()
        {
            return true;
        }

        protected override void ActivateStep()
        {
            _shopModel = _shopModelHolder.ShopModel;

            var casDeskBuildPointCellCoords = _shopModel.BuildPoints
                .First(p => p.Value.ShopObjectType == ShopObjectType.CashDesk)
                .Key;
            _targetPosition = _gridCalculator.GetCellCenterWorld(casDeskBuildPointCellCoords);
            
            _stepView = InstantiateColdPrefab<UITutorialTextStepView>(Constants.TutorialDefaultStepWithTextPath);
            
            _stepView.SetText(_localizationProvider.GetLocale(Constants.LocalizationTutorialBuildCashDeskMessageKey));
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            if (_stepView != null)
            {
                Destroy(_stepView);
                _stepView = null;
            }
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<PlayerCharPositionChangedEvent>(OnPlayerCharPositionChangedEvent);

            _shopModel.ShopObjectAdded += OnShopObjectAdded;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<PlayerCharPositionChangedEvent>(OnPlayerCharPositionChangedEvent);
            
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
        }

        private void OnShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            if (shopObjectModel.ShopObjectType == ShopObjectType.CashDesk)
            {
                DispatchStepFinished();
            }
        }

        private void OnPlayerCharPositionChangedEvent(PlayerCharPositionChangedEvent e)
        {
            var angle = Vector2.SignedAngle(_targetPosition - e.Position, new Vector2(0, -1));

            _stepView.SetArrowAngle(angle);
        }
    }
}