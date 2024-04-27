using UnityEngine;

namespace Infra.Instance
{
    internal abstract class InstanceHolder<T> where T : class
    {
        private static T _instance;

        public static T Get()
        {
#if UNITY_EDITOR
            Debug.Assert(_instance != null);
#endif

            return _instance;
        }

        public static void Set(T instance)
        {
            _instance = instance;
        }
    }
}