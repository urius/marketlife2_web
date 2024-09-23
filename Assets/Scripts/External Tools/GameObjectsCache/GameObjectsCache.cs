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
            
            var cache = GetCacheForPrefab(prefab);

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
                var capacity = cache.Capacity <= 0 ? _defaultCacheCapacityForPrefab : cache.Capacity;
                if (cache.Count < capacity)
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

        public void ClearCacheForPrefab(GameObject prefab)
        {
            if (_cacheByPrefabMap.TryGetValue(prefab, out var cache))
            {
                while (cache.TryGet(out var instance))
                {
                    Destroy(instance);
                }

                _cacheByPrefabMap.Remove(prefab);
            }
        }

        public void SetCacheCapacityForPrefab(GameObject prefab, int cacheCapacity)
        {
            var cache = GetCacheForPrefab(prefab);
            
            cache.SetCapacity(cacheCapacity);
        }

        private Cache GetCacheForPrefab(GameObject prefab)
        {
            _cacheByPrefabMap.TryAdd(prefab, new Cache());
            
            return _cacheByPrefabMap[prefab];
        }
        
        private class Cache
        {
            private readonly LinkedList<GameObject> _gameObjects = new();

            public int Capacity { get; private set; } = -1;
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

            public void SetCapacity(int capacity)
            {
                if (capacity > 0)
                {
                    Capacity = capacity;
                }
            }
        }
    }


    public interface IGameObjectsCache
    {
        public GameObject Get(GameObject prefab, Transform targetTransform);
        public void Put(GameObject instance);
        public void ClearCacheForPrefab(GameObject prefab);
        public void SetCacheCapacityForPrefab(GameObject prefab, int cacheCapacity);

    }
}