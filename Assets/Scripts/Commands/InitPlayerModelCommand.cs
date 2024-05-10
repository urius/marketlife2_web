using Cysharp.Threading.Tasks;
using Data.Dto;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Utils;

namespace Commands
{
    public struct InitPlayerModelCommand : IAsyncCommand<PlayerModelHolder>
    {
        public async UniTask ExecuteAsync(PlayerModelHolder playerModelHolder)
        {
            var commandExecutor = Instance.Get<ICommandExecutor>();
            
            var playerDataDto = await commandExecutor.ExecuteAsync<LoadOrCreatePlayerData, PlayerDataDto>();

            var model = playerDataDto.ToPlayerModel();

            playerModelHolder.SetModel(model);
        }
    }
}