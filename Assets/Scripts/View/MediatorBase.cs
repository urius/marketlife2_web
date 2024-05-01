using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using UnityEngine;
using UnityEngine.Assertions;

namespace View
{
    public abstract class MediatorBase
    {
        private LinkedList<MediatorBase> _children;
        private PrefabsHolderSo _prefabsHolder;
        private Transform _transform;
        
        protected Transform Transform => _transform;

        public void Mediate(Transform transform)
        {
            _transform = transform;

            MediateInternal();
        }
        
        public void Unmediate()
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
            
            _children ??= new LinkedList<MediatorBase>();
            
            var mediator = new T();
            mediator.Mediate(transform);
            
            _children.AddLast(mediator);
            
            return mediator;
        }

        protected GameObject InstantiatePrefab(PrefabKey prefabKey)
        {
            return InstantiatePrefab(prefabKey, _transform);
        }
        
        protected GameObject InstantiatePrefab(PrefabKey prefabKey, Transform transform)
        {
            _prefabsHolder ??= Instance.Get<PrefabsHolderSo>();

            return Object.Instantiate(_prefabsHolder.GetPrefabByKey(prefabKey), transform);
        }
    }
}