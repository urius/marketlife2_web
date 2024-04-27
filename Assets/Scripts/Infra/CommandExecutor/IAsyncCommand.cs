using Cysharp.Threading.Tasks;

namespace Infra.CommandExecutor
{
    public interface IAsyncCommand
    {
        UniTask ExecuteAsync();
    }

    public interface IAsyncCommand<in TArg>
    {
        UniTask ExecuteAsync(TArg arg);
    }

    public interface IAsyncCommandWithResult<TReturn>
    {
        UniTask<TReturn> ExecuteAsync();
    }

    public interface IAsyncCommandWithResult<TReturn, in TArg>
    {
        UniTask<TReturn> ExecuteAsync(TArg arg);
    }
}