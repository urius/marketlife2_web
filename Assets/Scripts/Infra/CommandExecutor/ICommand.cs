namespace Infra.CommandExecutor
{
    public interface ICommand
    {
        void Execute();
    }

    public interface ICommand<in TArg>
    {
        void Execute(TArg arg);
    }

    public interface ICommandWithResult<out TReturn>
    {
        TReturn Execute();
    }

    public interface ICommandWithResult<out TReturn, in TArg>
    {
        TReturn Execute(TArg parameter);
    }
}