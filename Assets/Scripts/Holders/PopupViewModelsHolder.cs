using System;
using System.Collections.Generic;
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
    }

    public interface IPopupViewModelsHolder
    {
        public event Action<PopupViewModelBase> PopupAdded;
        public event Action<PopupViewModelBase> PopupRemoved;
        
        public void AddPopup(PopupViewModelBase popupViewModel);
    }
}