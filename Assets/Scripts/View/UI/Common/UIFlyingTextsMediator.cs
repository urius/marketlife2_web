using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;

namespace View.UI.Common
{
    public class UIFlyingTextsMediator : MediatorBase
    {
        private const float TextFlyingTime = 1f;
        private const int FlyingTextYOffset = 100;
        
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IScreenCalculator _screenCalculator = Instance.Get<IScreenCalculator>();
        
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
            _eventBus.Subscribe<UIRequestFlyingTextEvent>(OnUIRequestFlyingTextEvent);
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<UIRequestFlyingTextEvent>(OnUIRequestFlyingTextEvent);
        }

        private void OnUIRequestFlyingTextEvent(UIRequestFlyingTextEvent e)
        {
            var flyingText = GetFromCache<UIFlyingTextView>(PrefabKey.UIFlyingText);
            flyingText.SetText(e.Text);

            SetTextColor(flyingText, e.Color);

            var screenPoint = _screenCalculator.WorldToScreenPoint(e.WorldPosition);
            flyingText.RectTransform.anchoredPosition = screenPoint;

            LeanTween.value(flyingText.gameObject, flyingText.SetAlpha, 1, 0, TextFlyingTime)
                .setEaseInQuad();
            
            flyingText.RectTransform
                .LeanMoveY(flyingText.RectTransform.anchoredPosition.y + FlyingTextYOffset, TextFlyingTime)
                .setEaseOutQuad()
                .setOnComplete(OnFlyingTextComplete)
                .setOnCompleteParam(flyingText);
        }

        private void SetTextColor(UIFlyingTextView flyingText, UIRequestFlyingTextColor color)
        {
            switch (color)
            {
                case UIRequestFlyingTextColor.Green:
                    flyingText.SetTextGreen();
                    break;
                case UIRequestFlyingTextColor.Red:
                    flyingText.SetTextRed();
                    break;
                case UIRequestFlyingTextColor.White:
                    flyingText.SetTextWhite();
                    break;
                case UIRequestFlyingTextColor.Default:
                    break;
            }
        }

        private void OnFlyingTextComplete(object flyingTextView)
        {
            var go = ((UIFlyingTextView)flyingTextView).gameObject;
            ReturnToCache(go);
        }
    }
}