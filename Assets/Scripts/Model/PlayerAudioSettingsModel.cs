using System;
using Tools.AudioManager;

namespace Model
{
    public class PlayerAudioSettingsModel : IAudioSettingsModel
    {
        public event Action<bool> AudioMutedStateChanged;
        public event Action<bool> MusicMutedStateChanged;
        
        public PlayerAudioSettingsModel(bool isAudioMuted, bool isMusicMuted)
        {
            IsAudioMuted = isAudioMuted;
            IsMusicMuted = isMusicMuted;
        }
            
        public bool IsAudioMuted { get; private set; }
        public bool IsMusicMuted { get; private set; }

        public void SetAudioMuted(bool isMuted)
        {
            IsAudioMuted = isMuted;
            
            AudioMutedStateChanged?.Invoke(IsAudioMuted);
        }

        public void SetMusicMuted(bool isMuted)
        {
            IsMusicMuted = isMuted;
            
            MusicMutedStateChanged?.Invoke(IsMusicMuted);
        }
    }
}