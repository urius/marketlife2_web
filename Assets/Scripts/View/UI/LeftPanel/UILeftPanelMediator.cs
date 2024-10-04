using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using Model.AdsOffer;
using UnityEngine;
using View.UI.LeftPanel.AdsOffer;

namespace View.UI.LeftPanel
{
    public class UILeftPanelMediator : MediatorBase
    {
        private readonly IAdsOfferViewModelsHolder _adsOfferViewModelsHolder = Instance.Get<IAdsOfferViewModelsHolder>();

        private readonly Dictionary<AdsOfferViewModelBase, MediatorBase> _adOfferMediators = new();
        
        protected override void MediateInternal()
        {
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _adsOfferViewModelsHolder.AdsOfferAdded += OnAdsOfferAdded;
            _adsOfferViewModelsHolder.AdsOfferRemoved += OnAdsOfferRemoved;
        }

        private void Unsubscribe()
        {
            _adsOfferViewModelsHolder.AdsOfferRemoved -= OnAdsOfferAdded;
            _adsOfferViewModelsHolder.AdsOfferRemoved -= OnAdsOfferRemoved;
        }

        private void OnAdsOfferAdded(AdsOfferViewModelBase viewModel)
        {
            MediatorBase mediator = null;
            
            switch (viewModel.AdsOfferType)
            {
                case AdsOfferType.AddMoney:
                    mediator = MediateChild<UIAdsOfferAddMoneyMediator, AdsOfferAddMoneyViewModel>(TargetTransform,
                        (AdsOfferAddMoneyViewModel)viewModel);
                    break;
                case AdsOfferType.MoneyMultiplier:
                    mediator = MediateChild<UIAdsOfferMoneyMultiplierMediator, AdsOfferMoneyMultiplierViewModel>(TargetTransform,
                        (AdsOfferMoneyMultiplierViewModel)viewModel);
                    break;
                case AdsOfferType.HireAllStaff:
                    mediator = MediateChild<UIAdsOfferHireAllStaffMediator, AdsOfferHireAllStaffViewModel>(TargetTransform,
                        (AdsOfferHireAllStaffViewModel)viewModel);
                    break;
                default:
                    Debug.LogError($"{nameof(OnAdsOfferAdded)}: unknown ads offer type {viewModel.AdsOfferType}");
                    break;
            }

            if (mediator != null)
            {
                _adOfferMediators[viewModel] = mediator;
            }
        }

        private void OnAdsOfferRemoved(AdsOfferViewModelBase viewModel)
        {
            if (_adOfferMediators.TryGetValue(viewModel, out var mediator))
            {
                mediator.Unmediate();
                
                _adOfferMediators.Remove(viewModel);
            }
        }
    }
}