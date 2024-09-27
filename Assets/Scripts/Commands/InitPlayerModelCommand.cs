using Cysharp.Threading.Tasks;
using Data.Dto;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Tools.AudioManager;
using Utils;

namespace Commands
{
    public struct InitPlayerModelCommand : IAsyncCommand<PlayerModelHolder>
    {
        public async UniTask ExecuteAsync(PlayerModelHolder playerModelHolder)
        {
            var commandExecutor = Instance.Get<ICommandExecutor>();
            var audioPlayer = Instance.Get<IAudioPlayer>();
            
            var playerDataDto = await commandExecutor.ExecuteAsync<LoadOrCreatePlayerData, PlayerDataDto>();

            var model = playerDataDto.ToPlayerModel();
            audioPlayer.SetSettings(model.AudioSettingsModel);
            playerModelHolder.SetModel(model);
        }
    }
}