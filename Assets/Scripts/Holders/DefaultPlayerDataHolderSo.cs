using Data.Dto;
using UnityEngine;

namespace Holders
{
    [CreateAssetMenu(fileName = "DefaultPlayerDataHolderSo", menuName = "ScriptableObjects/DefaultPlayerDataHolder")]
    public class DefaultPlayerDataHolderSo : ScriptableObject
    {
        [SerializeField] private PlayerDataDto _defaultPlayerData;

        public PlayerDataDto DefaultPlayerData => _defaultPlayerData;

        // [ContextMenu("test")]
        // public void Test()
        // {
        //     SetupInstance.From(this).AsSelf();
        //
        //     for (var i = 0; i < 10; i++)
        //     {
        //         var currentSize = 8 + i * 3;
        //         var l = ExpandShopHelper.GetXExpandLevel(currentSize);
        //         Debug.Log("expand X currentSize:" + currentSize + ", expand level: " + l);
        //     }
        // }
    }
}
