using System;

namespace Infra.EventBus
{
    public static class SubscriptionData<T>
    {
        public static Action<T> Action = delegate {  };
    }
}