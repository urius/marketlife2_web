using Data.Internal;
using Other;
using UnityEngine;

namespace Holders
{
    [CreateAssetMenu(fileName = "SoundsHolderSo", menuName = "ScriptableObjects/SoundsHolderSo")]
    public class SoundsHolderSo : ScriptableObject
    {
        [LabeledArray(nameof(SoundHolderSoItem.SoundIdKey))]
        [SerializeField] private SoundHolderSoItem[] _soundsCollection;

        public SoundHolderSoItem[] SoundsCollection => _soundsCollection;
    }
}