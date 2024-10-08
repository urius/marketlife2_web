using Data;
using Holders;
using Infra.Instance;
using UnityEngine;

namespace Utils
{
    public static class InstantiateHelper
    {
        public static T InstantiatePrefab<T>(PrefabKey prefabKey, Transform transform)
            where T : MonoBehaviour
        {
            var go = InstantiatePrefab(prefabKey, transform);
            var result = go.GetComponent<T>();
            return result;
        }
        
        public static GameObject InstantiatePrefab(PrefabKey prefabKey, Transform transform)
        {
            return Instantiate(GetPrefabByKey(prefabKey), transform);
        }
        
        public static GameObject Instantiate(GameObject prefab, Transform transform)
        {
            return Object.Instantiate(prefab, transform);
        }
        
        public static GameObject GetPrefabByKey(PrefabKey prefabKey)
        {
            var prefabsHolder = Instance.Get<PrefabsHolderSo>();

            return prefabsHolder.GetPrefabByKey(prefabKey);
        }
        
        public static void Destroy(MonoBehaviour monoBehaviour)
        {
            Destroy(monoBehaviour.gameObject);
        }
        
        public static void Destroy(GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }
    }
}