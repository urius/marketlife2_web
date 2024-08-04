using Holders;
using Infra.Instance;
using UnityEngine;

namespace View.UI.BottomPanel
{
    public abstract class UIBottomPanelMediatorBase<TView> : MediatorBase
        where TView : UIBottomPanelViewBase
    {
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();

        private float _slideUpPositionPercent = 0;
        private int _slideDirection = 1;
        
        protected TView PanelView { get; private set; }

        protected override void MediateInternal()
        {
            PanelView = TargetTransform.GetComponent<TView>();
            
            PanelView.SetSlideUpPositionPercent(_slideUpPositionPercent);
            PanelView.SetActive(false);        
        }

        protected void SlideUp()
        {
            _slideDirection = 1;
            
            PanelView.SetActive(true);
            
            ResubscribeOnSlideUpdate();
        }
        
        protected void SlideDown()
        {
            _slideDirection = -1;
            
            ResubscribeOnSlideUpdate();
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
            }

            if (_slideUpPositionPercent <= 0)
            {
                _slideUpPositionPercent = 0;
                _updatesProvider.GameplayFixedUpdate -= OnSlideGameplayFixedUpdate;
                
                PanelView.SetActive(false);
            }
            
            PanelView.SetSlideUpPositionPercent(_slideUpPositionPercent);
        }
    }
}