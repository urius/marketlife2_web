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

        protected Transform TargetTransform { get; private set; }

        public void Mediate(Transform transform)
        {
            TargetTransform = transform;

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
            return InstantiatePrefab(prefabKey, TargetTransform);
        }
        
        protected GameObject InstantiatePrefab(PrefabKey prefabKey, Transform transform)
        {
            _prefabsHolder ??= Instance.Get<PrefabsHolderSo>();

            return Object.Instantiate(_prefabsHolder.GetPrefabByKey(prefabKey), transform);
        }
    }
}