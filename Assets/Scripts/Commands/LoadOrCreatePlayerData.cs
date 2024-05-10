using Cysharp.Threading.Tasks;
using Data.Dto;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;

namespace Commands
{
    public struct LoadOrCreatePlayerData : IAsyncCommandWithResult<PlayerDataDto>
    {
        public UniTask<PlayerDataDto> ExecuteAsync()
        {
            //load data part
            //if data == null
            //then create new

            var result = Instance.Get<DefaultPlayerDataHolderSo>().DefaultPlayerData;
            
            return UniTask.FromResult(result);
        }
    }
}