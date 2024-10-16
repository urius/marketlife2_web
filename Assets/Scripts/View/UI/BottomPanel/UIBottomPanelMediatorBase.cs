using System;
using Data;
using Events;
using Extensions;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Tools.AudioManager;
using UnityEngine;

namespace View.UI.BottomPanel
{
    public abstract class UIBottomPanelMediatorBase<TView> : MediatorBase
        where TView : UIBottomPanelViewBase
    {
        protected event Action SlideUpFinished;
        protected event Action SlideDownFinished;
        
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private float _slideUpPositionPercent = 0;
        private int _slideDirection = 0;
        
        protected TView PanelView { get; private set; }

        protected override void MediateInternal()
        {
            PanelView = TargetTransform.GetComponent<TView>();
            
            PanelView.SetSlideUpPositionPercent(_slideUpPositionPercent);
            PanelView.SetActive(false);        
        }

        protected void SlideUp()
        {
            if (_slideDirection > 0) return;
            
            _slideDirection = 1;
            
            PanelView.SetActive(true);
            
            ResubscribeOnSlideUpdate();
        }
        
        protected void SlideDown()
        {
            if (_slideDirection < 0) return;
            
            _slideDirection = -1;
            
            ResubscribeOnSlideUpdate();
        }

        protected void PlayButtonSound()
        {
            _audioPlayer.PlaySound(SoundIdKey.Button_4);
        }

        protected void RequestFlyingText(string localeKey, Transform targetTransform)
        {
            _eventBus.Dispatch(
                new UIRequestFlyingTextEvent(
                    _localizationProvider.GetLocale(localeKey),
                    targetTransform.position));
        }

        private void ResubscribeOnSlideUpdate()
        {
            _updatesProvider.GameplayFixedUpdate -= OnSlideGameplayFixedUpdate;
            _updatesProvider.GameplayFixedUpdate += OnSlideGameplayFixedUpdate;
        }

        private void OnSlideGameplayFixedUpdate()
        {
            _slideUpPositionPercent += 5 * Time.fixedDeltaTime * _slideDirection;

            if (_slideUpPositionPercent >= 1)
            {
                _slideUpPositionPercent = 1;
                _updatesProvider.GameplayFixedUpdate -= OnSlideGameplayFixedUpdate;
                
                SlideUpFinished?.Invoke();
            }

            if (_slideUpPositionPercent <= 0)
            {
                _slideUpPositionPercent = 0;
                _updatesProvider.GameplayFixedUpdate -= OnSlideGameplayFixedUpdate;
                
                PanelView.SetActive(false);
                
                SlideDownFinished?.Invoke();
            }
            
            PanelView.SetSlideUpPositionPercent(_slideUpPositionPercent);
        }
    }
}