using System;

namespace Infra.EventBus
{
    public class EventBus : IEventBus
    {
        public void Dispatch<T>(T @event)
        {
            SubscriptionData<T>.Action?.Invoke(@event);
        }

        public void Subscribe<T>(Action<T> subscribeAction)
        {
            SubscriptionData<T>.Action += subscribeAction;
        }

        public void Unsubscribe<T>(Action<T> subscribeAction)
        {
            SubscriptionData<T>.Action -= subscribeAction;
        }

        public void UnsubscribeAll<T>()
        {
            SubscriptionData<T>.Action = null;
        }
    }
}