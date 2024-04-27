using System;
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

        public GameObject GetPrefabByKey(PrefabKey key)
        {
            return _items.First(i => i.Key == key).Prefab;
        }
        
        [Serializable]
        private struct PrefabHolderItem
        {
            public PrefabKey Key;
            public GameObject Prefab;
        }
    }
}
