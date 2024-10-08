using System;

namespace Tools.AudioManager
{
    public interface IAudioSettingsModel
    {
        public event Action<bool> SoundsMutedStateChanged;
        public event Action<bool> MusicMutedStateChanged;
        
        public bool IsSoundsMuted { get; }
        public bool IsMusicMuted { get; }
    }
}