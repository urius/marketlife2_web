using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Other;
using UnityEngine;

namespace Holders
{
    [CreateAssetMenu(fileName = "PrefabsHolderSo", menuName = "ScriptableObjects/PrefabsHolderSo")]
    public class PrefabsHolderSo : ScriptableObject
    {
        [LabeledArray(nameof(PrefabHolderItem.Key))] [SerializeField] private PrefabHolderItem[] _items;

        private readonly Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>();
        
        public PrefabHolderItem[] Items => _items;

        public GameObject GetPrefabByKey(PrefabKey key)
        {
            return _items.First(i => i.Key == key).Prefab;
        }
        
        public GameObject GetColdPrefab(string coldPrefabPath)
        {
            if (_prefabCache.TryGetValue(coldPrefabPath, out var coldPrefab))
            {
                return coldPrefab;
            }
            
            var prefab = Resources.Load<GameObject>(coldPrefabPath);
            if (prefab != null)
            {
                _prefabCache.Add(coldPrefabPath, prefab);
            }
            else
            {
                Debug.LogError("Prefab not found at path: " + coldPrefabPath);
            }

            return _prefabCache[coldPrefabPath];
        }

        public void UnloadColdPrefabs()
        {
            Resources.UnloadUnusedAssets();
            
            _prefabCache.Clear();
        }

        [Serializable]
        public struct PrefabHolderItem
        {
            public PrefabKey Key;
            public GameObject Prefab;
        }
    }
}
