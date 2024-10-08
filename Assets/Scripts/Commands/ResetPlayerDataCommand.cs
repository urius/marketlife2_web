using Infra.CommandExecutor;
using Tools;

namespace Commands
{
    public class ResetPlayerDataCommand : ICommand
    {
        public void Execute()
        {
            GamePushWrapper.ResetPlayer();
        }
    }
}