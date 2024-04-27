using System;

namespace Infra.EventBus
{
    internal static class SubscriptionData<T>
    {
        public static Action<T> Action;
    }
}