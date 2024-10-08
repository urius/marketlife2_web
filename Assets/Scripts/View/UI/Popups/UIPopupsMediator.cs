using System;
using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using Model.Popups;
using UnityEngine;
using View.UI.Popups.InteriorPopup;
using View.UI.Popups.SettingsPopup;

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
            SetCanvasActivity(true);
            
            switch (popupViewModel.PopupKey)
            {
                case PopupKey.InteriorPopup:
                    MediatePopupInternal<UIInteriorPopupMediator, InteriorPopupViewModel>(popupViewModel);
                    break;
                case PopupKey.SettingsPopup:
                    MediatePopupInternal<UISettingsPopupController, SettingsPopupViewModel>(popupViewModel);
                    break;
                default:
                    UpdateCanvasActivity();
                    throw new NotSupportedException(
                        $"{nameof(UIPopupsMediator)}: Unsupported popup key: {popupViewModel.PopupKey}");
            }
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
            SetCanvasActivity(_popupMediatorsByModel.Count > 0);
        }

        private void SetCanvasActivity(bool isActive)
        {
            _targetCanvas.enabled = isActive;
            _targetCanvas.gameObject.SetActive(isActive);
        }
    }
}