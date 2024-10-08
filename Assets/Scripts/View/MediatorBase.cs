using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using Tools.GameObjectsCache;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;

namespace View
{
    public abstract class MediatorBase
    {
        private IGameObjectsCache _gameObjectsCache;
        private LinkedList<MediatorBase> _children;
        private PrefabsHolderSo _prefabsHolder;

        protected Transform TargetTransform { get; private set; }

        public virtual void Mediate(Transform transform)
        {
            TargetTransform = transform;

            MediateInternal();
        }
        
        public virtual void Unmediate()
        {
            if (_children != null)
            {
                foreach (var childMediator in _children)
                {
                    childMediator.Unmediate();
                }
                _children.Clear();
            }

            UnmediateInternal();
        }
        
        protected abstract void MediateInternal();
        protected abstract void UnmediateInternal();

        protected T MediateChild<T>(Transform transform) where T : MediatorBase, new()
        {
            Assert.IsNotNull(transform);
            
            var mediator = new T();
            mediator.Mediate(transform);
            
            AddChildMediator(mediator);

            return mediator;
        }

        protected T MediateChild<T, TModel>(Transform transform, TModel model)
            where TModel : class
            where T : MediatorWithModelBase<TModel>, new()
        {
            Assert.IsNotNull(transform);
            Assert.IsNotNull(model);

            var mediator = new T();
            mediator.Mediate(transform, model);

            AddChildMediator(mediator);

            return mediator;
        }

        protected MediatorBase MediateChild(MediatorBase mediator, Transform transform)
        {
            Assert.IsNotNull(transform);
            
            mediator.Mediate(transform);
            
            AddChildMediator(mediator);

            return mediator;
        }
        
        protected MediatorBase MediateChild(MediatorBase mediator)
        {
            return MediateChild(mediator, TargetTransform);
        }

        protected bool UnmediateChild(MediatorBase childMediator)
        {
            if (_children.Contains(childMediator))
            {
                childMediator.Unmediate();
                _children.Remove(childMediator);

                return true;
            }

            return false;
        }

        protected void AddChildMediator(MediatorBase mediator)
        {
            _children ??= new LinkedList<MediatorBase>();
            _children.AddLast(mediator);
        }

        protected T InstantiatePrefab<T>(PrefabKey prefabKey)
            where T : MonoBehaviour
        {
            return InstantiatePrefab<T>(prefabKey, TargetTransform);
        }

        protected T InstantiatePrefab<T>(PrefabKey prefabKey, Transform transform)
            where T : MonoBehaviour
        {
            return InstantiateHelper.InstantiatePrefab<T>(prefabKey, transform);
        }

        protected GameObject InstantiatePrefab(PrefabKey prefabKey)
        {
            return InstantiatePrefab(prefabKey, TargetTransform);
        }
        
        protected GameObject InstantiatePrefab(PrefabKey prefabKey, Transform transform)
        {
            return InstantiateHelper.InstantiatePrefab(prefabKey, transform);
        }

        protected static GameObject Instantiate(GameObject prefab, Transform transform)
        {
            return InstantiateHelper.Instantiate(prefab, transform);
        }

        protected T GetComponentInPrefab<T>(PrefabKey prefabKey)
            where T : MonoBehaviour
        {
            var prefabGo = GetPrefabByKey(prefabKey);
            
            return prefabGo.GetComponent<T>();
        } 
        
        protected void Destroy(MonoBehaviour monoBehaviour)
        {
            InstantiateHelper.Destroy(monoBehaviour);
        }

        protected void Destroy(GameObject gameObject)
        {
            InstantiateHelper.Destroy(gameObject);
        }

        protected GameObject GetFromCache(PrefabKey prefabKey, Transform transform)
        {
            _gameObjectsCache ??= Instance.Get<IGameObjectsCache>();

            var prefab = GetPrefabByKey(prefabKey);

            return _gameObjectsCache.Get(prefab, transform);
        }
        
        protected GameObject GetFromCache(PrefabKey prefabKey)
        {
            return GetFromCache(prefabKey, TargetTransform);
        }

        protected T GetFromCache<T>(PrefabKey prefabKey, Transform transform)
            where T : MonoBehaviour
        {
            var instance = GetFromCache(prefabKey, transform);
            var component = instance.GetComponent<T>();

            return component;
        }

        protected T GetFromCache<T>(PrefabKey prefabKey)
            where T : MonoBehaviour
        {
            return GetFromCache<T>(prefabKey, TargetTransform);
        }

        protected void ReturnToCache(GameObject instance)
        {
            _gameObjectsCache ??= Instance.Get<IGameObjectsCache>();
            _gameObjectsCache.Put(instance);
        }

        protected void ClearCache(PrefabKey prefabKey)
        {
            _gameObjectsCache ??= Instance.Get<IGameObjectsCache>();

            var prefab = GetPrefabByKey(prefabKey);
            _gameObjectsCache.ClearCacheForPrefab(prefab);
        }

        protected void SetPrefabCacheCapacity(PrefabKey prefabKey, int capacity)
        {
            _gameObjectsCache ??= Instance.Get<IGameObjectsCache>();
            
            var prefab = GetPrefabByKey(prefabKey);
            _gameObjectsCache.SetCacheCapacityForPrefab(prefab, capacity);
        }

        protected GameObject InstantiateColdPrefab(string path)
        {
            _prefabsHolder ??= Instance.Get<PrefabsHolderSo>();
            
            var prefab = _prefabsHolder.GetColdPrefab(path);
            if (prefab != null)
            {
                return Instantiate(prefab, TargetTransform);
            }

            return null;
        }

        protected TView InstantiateColdPrefab<TView>(string path) where TView : MonoBehaviour
        {
            var go = InstantiateColdPrefab(path);

            if (go != null)
            {
                return go.GetComponent<TView>();
            }

            return null;
        }

        private GameObject GetPrefabByKey(PrefabKey prefabKey)
        {
            return InstantiateHelper.GetPrefabByKey(prefabKey);
        }
    }
}