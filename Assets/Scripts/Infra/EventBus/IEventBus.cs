using System;

namespace Infra.EventBus
{
    public interface IEventBus
    {
        void Dispatch<T>(T @event);

        void Subscribe<T>(Action<T> subscribeAction);
        void Unsubscribe<T>(Action<T> subscribeAction);
        void UnsubscribeAll<T>();
    }
}