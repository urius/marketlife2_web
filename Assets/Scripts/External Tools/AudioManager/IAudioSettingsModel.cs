using System;

namespace Tools.AudioManager
{
    public interface IAudioSettingsModel
    {
        public event Action<bool> AudioMutedStateChanged;
        public event Action<bool> MusicMutedStateChanged;
        
        public bool IsAudioMuted { get; }
        public bool IsMusicMuted { get; }
    }
}