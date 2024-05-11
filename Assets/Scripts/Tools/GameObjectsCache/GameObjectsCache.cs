using System.Collections.Generic;
using UnityEngine;

namespace Tools.GameObjectsCache
{
    public class GameObjectsCache : MonoBehaviour, IGameObjectsCache
    {
        [SerializeField] private int _defaultCacheCapacityForPrefab;
        [SerializeField] private Transform _cacheContainerTransform;
        
        private readonly Dictionary<GameObject, GameObject> _prefabByInstanceMap = new();
        private readonly Dictionary<GameObject, Cache> _cacheByPrefabMap = new();

        private void Awake()
        {
            _cacheContainerTransform.gameObject.SetActive(false);
        }

        public GameObject Get(GameObject prefab, Transform targetTransform)
        {
            GameObject result;
            
            _cacheByPrefabMap.TryAdd(prefab, new Cache());
            var cache = _cacheByPrefabMap[prefab];
            
            if (cache.TryGet(out var instance))
            {
                instance.transform.SetParent(targetTransform);
                result = instance;
            }
            else
            {
                result = Instantiate(prefab, targetTransform);
                _prefabByInstanceMap[result] = prefab;
            }

            return result;
        }
        
        public void Put(GameObject instance)
        {
            if (_prefabByInstanceMap.TryGetValue(instance, out var prefab)
                && _cacheByPrefabMap.TryGetValue(prefab, out var cache))
            {
                if (cache.Count < _defaultCacheCapacityForPrefab)
                {
                    instance.transform.SetParent(_cacheContainerTransform);
                    cache.Put(instance);

                    return;
                }
            }
            else
            {
                Debug.LogError(
                    $"{nameof(GameObjectsCache)}: Trying to put instance of unknown prefab (that hasn't been taken via Get before), instance name = {instance.name}");
            }
            
            Destroy(instance);
        }
        
        private class Cache
        {
            private readonly LinkedList<GameObject> _gameObjects = new();

            public int Count => _gameObjects.Count;
            
            public bool TryGet(out GameObject instance)
            {
                instance = null;
                
                var isExist = _gameObjects.Count > 0;
                if (isExist)
                {
                    instance = _gameObjects.Last.Value;
                    _gameObjects.RemoveLast();
                }

                return isExist;
            }

            public void Put(GameObject instance)
            {
                _gameObjects.AddLast(instance);
            }
        }
    }


    public interface IGameObjectsCache
    {
        public GameObject Get(GameObject prefab, Transform targetTransform);
        public void Put(GameObject instance);
    }
}