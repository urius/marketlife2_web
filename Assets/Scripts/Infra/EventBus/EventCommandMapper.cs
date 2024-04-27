using Infra.CommandExecutor;

namespace Infra.EventBus
{
    public class EventCommandMapper
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IEventBus _eventBus;

        public EventCommandMapper(IEventBus eventBus, ICommandExecutor commandExecutor)
        {
            _eventBus = eventBus;
            _commandExecutor = commandExecutor;
        }

        public void Map<TEvent, TCommand>()
            where TCommand : ICommand<TEvent>, new()
        {
            _eventBus.Subscribe<TEvent>(ExecuteEventSyncCommandAction<TCommand, TEvent>);
        }

        public void Unmap<TEvent, TCommand>()
            where TCommand : ICommand<TEvent>, new()
        {
            _eventBus.Unsubscribe<TEvent>(ExecuteEventSyncCommandAction<TCommand, TEvent>);
        }

        private void ExecuteEventSyncCommandAction<TCommand, TEvent>(TEvent @event)
            where TCommand : ICommand<TEvent>, new()
        {
            _commandExecutor.Execute<TCommand, TEvent>(@event);
        }

        private void ExecuteEventSyncCommandActionNoParams<TEvent, TCommand>(TEvent @event)
            where TCommand : ICommand, new()
        {
            _commandExecutor.Execute<TCommand>();
        }
    }
}