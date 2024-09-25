using Data;
using Holders;
using Infra.Instance;
using Model;
using Model.People;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepMoveToCashDeskMediator : UITutorialStepMoveToMediatorBase
    {
        private const ShopCharStateName TargetStateName = ShopCharStateName.CustomerMovingToCashDesk;
        
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private ShopModel _shopModel;
        private CustomerCharModel _targetCustomer;

        protected override string MessageText =>
            _localizationProvider.GetLocale(Constants.LocalizationTutorialMoveToCashDeskMessageKey);

        protected override void MediateInternal()
        {
            _shopModel = _shopModelHolder.ShopModel;
            
            base.MediateInternal();
        }

        protected override bool CheckStepConditions()
        {
            var waitingCustomer = GetWaitingCustomer();
            
            return waitingCustomer != null;
        }

        protected override void ActivateStep()
        {
            _targetCustomer = GetWaitingCustomer();
            
            base.ActivateStep();
        }

        protected override void Subscribe()
        {
            base.Subscribe();

            _updatesProvider.SecondPassed += OnSecondPassed;
        }

        protected override void Unsubscribe()
        {
            _updatesProvider.SecondPassed -= OnSecondPassed;

            base.Unsubscribe();
        }

        private void OnSecondPassed()
        {
            if (_targetCustomer.State.StateName != TargetStateName)
            {
                DispatchStepFinished();
            }
        }

        private CustomerCharModel GetWaitingCustomer()
        {
            return _shopModel.CustomersModel.GetWaitingCustomer();
        }

        protected override Vector2Int GetTargetMoveToCell()
        {
            return _shopModel.CashDesks[0].CellCoords;
        }
    }
}