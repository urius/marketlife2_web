using System;
using System.Diagnostics;
using UnityEngine.Assertions;

namespace Infra.Instance
{
    public static class SetupInstance
    {
        internal static Type UnfinishedSetupType;
        
        public static SetupInstanceContext<T> From<T>(T instance) where T : class
        {
            AssertUnfinishedSetups();
            UnfinishedSetupType = typeof(T);
            
            return new SetupInstanceContext<T>(instance);
        }

        public static void AllSetupsDone()
        {
            AssertUnfinishedSetups();
        }

        [Conditional("UNITY_EDITOR")]
        private static void AssertUnfinishedSetups()
        {
            Assert.IsNull(UnfinishedSetupType, $"UnfinishedSetup detected, type = {UnfinishedSetupType}");
        }
    }

    public class SetupInstanceContext<T> where T : class
    {
        private readonly T _instance;

        public SetupInstanceContext(T instance)
        {
            _instance = instance;
        }

        public SetupInstanceContext<T> As<TInterface>() where TInterface : class
        {
            if (_instance is TInterface castedInstance)
            {
                InstanceHolder<TInterface>.Set(castedInstance);
                
                SetupInstance.UnfinishedSetupType = null;
                
                return this;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Instance of {nameof(T)} cannot be assigned to the specified type {nameof(TInterface)}");
            }
        }
        
        public SetupInstanceContext<T> AsSelf()
        {
            return As<T>();
        }
    }
}