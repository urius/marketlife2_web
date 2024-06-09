using System.Collections.Generic;
using Holders;
using Infra.Instance;

namespace Systems
{
    public class RootSystem : ISystem
    {
        private IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();

        private readonly LinkedList<ISystem> _systems = new();
        
        public async void Start()
        {
            await _playerModelHolder.PlayerModelSetTask;

            StartSystem<PlayerCharSystem>();
            StartSystem<BuildPointsAppearanceSystem>();
        }

        public void Stop()
        {
            foreach (var system in _systems)
            {
                system.Stop();
            }

            _systems.Clear();
        }

        private void StartSystem<T>()
            where T : ISystem, new()
        {
            var system = new T();
            system.Start();

            _systems.AddLast(system);
        }

        private void StopSystem<T>()
            where T : ISystem, new()
        {
            foreach (var system in _systems)
            {
                if (system is T targetSystem)
                {
                    targetSystem.Stop();
                    _systems.Remove(targetSystem);
                    break;
                }
            }
        }
    }
}