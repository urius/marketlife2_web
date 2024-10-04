using System.Collections.Generic;
using Holders;
using Infra.Instance;
using Systems.Tutorial;

namespace Systems
{
    public class RootSystem : ISystem
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();

        private readonly LinkedList<ISystem> _systems = new();
        
        public async void Start()
        {
            StartSystem<GamePauseSystem>();
            
            await _playerModelHolder.PlayerModelSetTask;

            StartSystem<PlayerLevelSystem>();
            StartSystem<PlayerModifiersSystem>();
            StartSystem<PlayerCharSystem>();
            StartSystem<BuildPointsAppearanceSystem>();
            StartSystem<TruckPointsLogicSystem>();
            StartSystem<CustomersControlSystem>();
            StartSystem<StaffControlSystem>();
            StartSystem<TutorialSystem>();
            StartSystem<AdsOfferSystem>();
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