using System;
using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using Model.Popups;
using UnityEngine;

namespace View.UI.Popups
{
    public class UIPopupsMediator : MediatorBase
    {
        private readonly IPopupViewModelsHolder _popupViewModelsHolder = Instance.Get<IPopupViewModelsHolder>();
        private readonly Dictionary<PopupViewModelBase, MediatorBase> _popupMediatorsByModel = new();

        private Canvas _targetCanvas;

        protected override void MediateInternal()
        {
            _targetCanvas = TargetTransform.GetComponent<Canvas>();

            UpdateCanvasActivity();
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();

            var popupModels = _popupMediatorsByModel.Keys;
            foreach (var popupModel in popupModels)
            {
                UnmediatePopup(popupModel);
            }
            
            _popupMediatorsByModel.Clear();
        }

        private void Subscribe()
        {
            _popupViewModelsHolder.PopupAdded += OnPopupAdded;
            _popupViewModelsHolder.PopupRemoved += OnPopupRemoved;
        }

        private void Unsubscribe()
        {
            _popupViewModelsHolder.PopupAdded -= OnPopupAdded;
            _popupViewModelsHolder.PopupRemoved -= OnPopupRemoved;
        }

        private void OnPopupAdded(PopupViewModelBase popupViewModel)
        {
            MediatePopup(popupViewModel);
        }

        private void OnPopupRemoved(PopupViewModelBase popupViewModel)
        {
            UnmediatePopup(popupViewModel);
        }

        private void MediatePopup(PopupViewModelBase popupViewModel)
        {
            switch (popupViewModel.PopupKey)
            {
                case PopupKey.InteriorPopup:
                    MediatePopupInternal<UIInteriorPopupMediator, InteriorPopupViewModel>(popupViewModel);
                    break;
                default:
                    throw new NotSupportedException(
                        $"{nameof(UIPopupsMediator)}: Unsupported popup key: {popupViewModel.PopupKey}");
            }

            UpdateCanvasActivity();
        }

        private void MediatePopupInternal<TMediator, TViewModel>(PopupViewModelBase popupViewModel)
            where TViewModel : PopupViewModelBase
            where TMediator : MediatorWithModelBase<TViewModel>, new()
        {
            var mediator = MediateChild<TMediator, TViewModel>(TargetTransform, (TViewModel)popupViewModel);
            _popupMediatorsByModel[popupViewModel] = mediator;
        }
        
        private void UnmediatePopup(PopupViewModelBase popupViewModel)
        {
            if (_popupMediatorsByModel.ContainsKey(popupViewModel))
            {
                var mediator = _popupMediatorsByModel[popupViewModel];
                UnmediateChild(mediator);
                
                _popupMediatorsByModel.Remove(popupViewModel);
            
                UpdateCanvasActivity();
            }
        }

        private void UpdateCanvasActivity()
        {
            var openedPopupsCount = _popupMediatorsByModel.Count;
            var isActive = openedPopupsCount > 0;
            
            _targetCanvas.enabled = isActive;
            _targetCanvas.gameObject.SetActive(isActive);
        }
    }
}