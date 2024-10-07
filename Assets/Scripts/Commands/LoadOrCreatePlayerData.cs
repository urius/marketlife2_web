using Cysharp.Threading.Tasks;
using Data;
using Data.Dto;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Tools;
using UnityEngine;

namespace Commands
{
    public struct LoadOrCreatePlayerData : IAsyncCommandWithResult<PlayerDataDto>
    {
        public UniTask<PlayerDataDto> ExecuteAsync()
        {
            PlayerDataDto result;
            
            var playerDataStr = GamePushWrapper.GetPlayerData(Constants.PlayerDataKey);

            Debug.Log($"{nameof(GamePushWrapper.GetPlayerData)} result: \n {playerDataStr}");
            
            if (string.IsNullOrEmpty(playerDataStr))
            {
                result = Instance.Get<DefaultPlayerDataHolderSo>().DefaultPlayerData;
            }
            else
            {
                result = JsonUtility.FromJson<PlayerDataDto>(playerDataStr);
            }
            
            return UniTask.FromResult(result);
        }
    }
}