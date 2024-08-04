using Commands;
using Events;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;

namespace Systems
{
    public class ProcessCommandsSystem : ISystem
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        public void Start()
        {
            MapEvent<UIShelfUpgradeClickedEvent, UpgradeShelfCommand>();
        }

        public void Stop()
        {
            UnmapEvent<UIShelfUpgradeClickedEvent, UpgradeShelfCommand>();
        }

        private void MapEvent<TEvent, TCommand>()
            where TCommand : ICommand<TEvent>, new()
        {
            _eventBus.Subscribe<TEvent>(EventHandler<TEvent, TCommand>);
        }

        private void UnmapEvent<TEvent, TCommand>()
            where TCommand : ICommand<TEvent>, new()
        {
            _eventBus.Unsubscribe<TEvent>(EventHandler<TEvent, TCommand>);
        }

        private static void EventHandler<TEvent, TCommand>(TEvent e)
            where TCommand : ICommand<TEvent>, new()
        {
            new TCommand().Execute(e);
        }
    }
}