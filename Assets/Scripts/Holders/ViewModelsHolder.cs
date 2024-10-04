using System;
using System.Collections.Generic;
using Data;
using Model.AdsOffer;
using Model.Popups;

namespace Holders
{
    public class ViewModelsHolder : IPopupViewModelsHolder, IAdsOfferViewModelsHolder
    {
        public event Action<PopupViewModelBase> PopupAdded;
        public event Action<PopupViewModelBase> PopupRemoved;
        public event Action<AdsOfferViewModelBase> AdsOfferAdded;
        public event Action<AdsOfferViewModelBase> AdsOfferRemoved;

        private readonly LinkedList<PopupViewModelBase> _popupsStack = new();

        public AdsOfferViewModelBase CurrentAdsOfferViewModel { get; private set; }

        public void AddPopup(PopupViewModelBase popupViewModel)
        {
            _popupsStack.AddLast(popupViewModel);
            
            PopupAdded?.Invoke(popupViewModel);
        }

        public void RemovePopup(PopupViewModelBase popupViewModel)
        {
            if (_popupsStack.Contains(popupViewModel))
            {
                _popupsStack.Remove(popupViewModel);
                
                PopupRemoved?.Invoke(popupViewModel);
            }
        }

        public PopupViewModelBase FindPopupByKey(PopupKey popupKey)
        {
            foreach (var popupViewModel in _popupsStack)
            {
                if (popupViewModel.PopupKey == popupKey)
                {
                    return popupViewModel;
                }
            }

            return null;
        }
        
        public void SetAdsOffer(AdsOfferViewModelBase viewModel)
        {
            if (CurrentAdsOfferViewModel != null)
            {
                RemoveCurrentAdsOffer();
            }

            CurrentAdsOfferViewModel = viewModel;
            
            AdsOfferAdded?.Invoke(CurrentAdsOfferViewModel);
        }

        public bool RemoveCurrentAdsOffer()
        {
            var removedViewModel = CurrentAdsOfferViewModel;

            if (CurrentAdsOfferViewModel != null)
            {
                CurrentAdsOfferViewModel = null;
                
                AdsOfferRemoved?.Invoke(removedViewModel);
            }

            return removedViewModel != null;
        }
    }

    public interface IPopupViewModelsHolder
    {
        public event Action<PopupViewModelBase> PopupAdded;
        public event Action<PopupViewModelBase> PopupRemoved;
        
        public void AddPopup(PopupViewModelBase popupViewModel);
        public PopupViewModelBase FindPopupByKey(PopupKey popupKey);
        public void RemovePopup(PopupViewModelBase popupViewModel);
    }

    public interface IAdsOfferViewModelsHolder
    {
        public event Action<AdsOfferViewModelBase> AdsOfferAdded;
        public event Action<AdsOfferViewModelBase> AdsOfferRemoved;
        
        public AdsOfferViewModelBase CurrentAdsOfferViewModel { get; }
        
        public void SetAdsOffer(AdsOfferViewModelBase viewModel);
        public bool RemoveCurrentAdsOffer();
    }
}