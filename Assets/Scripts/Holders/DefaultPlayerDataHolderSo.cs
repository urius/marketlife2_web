using Data;
using UnityEngine;

namespace Holders
{
    [CreateAssetMenu(fileName = "DefaultPlayerDataHolderSo", menuName = "ScriptableObjects/DefaultPlayerDataHolder")]
    public class DefaultPlayerDataHolderSo : ScriptableObject
    {
        [SerializeField] private PlayerDataDto _defaultPlayerData;

        public PlayerDataDto DefaultPlayerData => _defaultPlayerData;
    }
}
