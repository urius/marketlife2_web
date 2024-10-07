using System.Collections.Generic;
using Data;
using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Tools.AudioManager;
using UnityEngine;

namespace Systems
{
    public class GamePauseSystem : ISystem
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        private readonly ISharedFlagsHolder _sharedFlagsHolder = Instance.Get<ISharedFlagsHolder>();

        private readonly LinkedList<PauseRequesterData> _pauseRequesters = new ();

        private bool IsGamePaused => _pauseRequesters.Count > 0;
        
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
            if (e.IsPaused && HasPauseRequester(e.RequesterId) == false)
            {
                var requesterData = new PauseRequesterData(e.RequesterId, e.NeedMute);
                _pauseRequesters.AddLast(requesterData);
            }
            else if (e.IsPaused == false)
            {
                RemoveRequester(e.RequesterId);
            }

            var isPaused = IsGamePaused; 
            _sharedFlagsHolder.Set(SharedFlagKey.IsGamePaused, isPaused);
            
            Time.timeScale = isPaused ? 0 : 1;
            
            UpdateMuteState();
        }

        private void RemoveRequester(string requesterId)
        {
            foreach (var pauseRequester in _pauseRequesters)
            {
                if (pauseRequester.RequesterId == requesterId)
                {
                    _pauseRequesters.Remove(pauseRequester);
                    return;
                }
            }
        }

        private void UpdateMuteState()
        {
            foreach (var pauseRequester in _pauseRequesters)
            {
                if (pauseRequester.NeedMute)
                {
                    _audioPlayer.MuteBy(nameof(GamePauseSystem));
                    return;
                }
            }
            
            _audioPlayer.UnmuteBy(nameof(GamePauseSystem));
        }

        private bool HasPauseRequester(string requesterId)
        {
            foreach (var pauseRequester in _pauseRequesters)
            {
                if (pauseRequester.RequesterId == requesterId)
                {
                    return true;
                }
            }

            return false;
        }
        
        private struct PauseRequesterData
        {
            public readonly string RequesterId;
            public readonly bool NeedMute;

            public PauseRequesterData(string requesterId, bool needMute)
            {
                RequesterId = requesterId;
                NeedMute = needMute;
            }
        }
    }
}