using Cysharp.Threading.Tasks;

namespace Infra.CommandExecutor
{
    public class CommandExecutor : ICommandExecutor
    {
        public UniTask ExecuteAsync<T>() where T : IAsyncCommand, new()
        {
            return new T().ExecuteAsync();
        }

        public UniTask ExecuteAsync<T, TArg>(TArg arg) where T : IAsyncCommand<TArg>, new()
        {
            return new T().ExecuteAsync(arg);
        }

        public UniTask<TReturn> ExecuteAsync<T, TReturn>() where T : IAsyncCommandWithResult<TReturn>, new()
        {
            return new T().ExecuteAsync();
        }

        public UniTask<TReturn> ExecuteAsync<T, TReturn, TArg>(TArg argument)
            where T : IAsyncCommandWithResult<TReturn, TArg>, new()
        {
            return new T().ExecuteAsync(argument);
        }

        public void Execute<T>() where T : ICommand, new()
        {
            new T().Execute();
        }

        public void Execute<T, TArg>(TArg arg) where T : ICommand<TArg>, new()
        {
            new T().Execute(arg);
        }

        public TReturn Execute<T, TReturn>() where T : ICommandWithResult<TReturn>, new()
        {
            return new T().Execute();
        }

        public TReturn Execute<T, TReturn, TArg>(TArg argument) where T : ICommandWithResult<TReturn, TArg>, new()
        {
            return new T().Execute(argument);
        }
    }
}