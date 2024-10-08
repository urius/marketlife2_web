using Cysharp.Threading.Tasks;

namespace Infra.CommandExecutor
{
    public interface ICommandExecutor
    {
        UniTask ExecuteAsync<T>() where T : IAsyncCommand, new();
        UniTask ExecuteAsync<T, TArg>(TArg arg) where T : IAsyncCommand<TArg>, new();
        UniTask<TReturn> ExecuteAsync<T, TReturn>() where T : IAsyncCommandWithResult<TReturn>, new();
        UniTask<TReturn> ExecuteAsync<T, TReturn, TArg>(TArg argument)
            where T : IAsyncCommandWithResult<TReturn, TArg>, new();

        void Execute<T>() where T : ICommand, new();
        void Execute<T, TArg>(TArg arg) where T : ICommand<TArg>, new();
        TReturn Execute<T, TReturn>() where T : ICommandWithResult<TReturn>, new();
        TReturn Execute<T, TReturn, TArg>(TArg argument) where T : ICommandWithResult<TReturn, TArg>, new();
    }
}