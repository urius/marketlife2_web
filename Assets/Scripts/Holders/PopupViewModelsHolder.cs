using System;
using System.Collections.Generic;
using Data;
using Model.Popups;

namespace Holders
{
    public class PopupViewModelsHolder : IPopupViewModelsHolder
    {
        public event Action<PopupViewModelBase> PopupAdded;
        public event Action<PopupViewModelBase> PopupRemoved;

        private readonly LinkedList<PopupViewModelBase> _popupsStack = new();
        
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
    }

    public interface IPopupViewModelsHolder
    {
        public event Action<PopupViewModelBase> PopupAdded;
        public event Action<PopupViewModelBase> PopupRemoved;
        
        public void AddPopup(PopupViewModelBase popupViewModel);
        public PopupViewModelBase FindPopupByKey(PopupKey popupKey);
    }
}