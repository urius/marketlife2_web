using System.Collections.Generic;
using Events;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;

namespace Systems
{
    public class GamePauseSystem : ISystem
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();

        private readonly LinkedList<string> _pauseRequesters = new ();
        
        public void Start()
        {
            _eventBus.Subscribe<RequestGamePauseEvent>(OnRequestGamePauseEvent);
        }

        public void Stop()
        {
            _eventBus.Unsubscribe<RequestGamePauseEvent>(OnRequestGamePauseEvent);
        }

        private void OnRequestGamePauseEvent(RequestGamePauseEvent e)
        {
            if (e.IsPaused && _pauseRequesters.Contains(e.RequesterId) == false)
            {
                _pauseRequesters.AddLast(e.RequesterId);
            }
            else if (e.IsPaused == false)
            {
                _pauseRequesters.Remove(e.RequesterId);
            }

            Time.timeScale = _pauseRequesters.Count > 0 ? 0 : 1;
        }
    }
}