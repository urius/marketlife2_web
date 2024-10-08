using System;
using Tools.AudioManager;

namespace Model
{
    public class PlayerAudioSettingsModel : IAudioSettingsModel
    {
        public event Action<bool> SoundsMutedStateChanged;
        public event Action<bool> MusicMutedStateChanged;
        
        public PlayerAudioSettingsModel(bool isAudioMuted, bool isMusicMuted)
        {
            IsSoundsMuted = isAudioMuted;
            IsMusicMuted = isMusicMuted;
        }
            
        public bool IsSoundsMuted { get; private set; }
        public bool IsMusicMuted { get; private set; }

        public void SetSoundsMuted(bool isMuted)
        {
            IsSoundsMuted = isMuted;
            
            SoundsMutedStateChanged?.Invoke(IsSoundsMuted);
        }

        public void SetMusicMuted(bool isMuted)
        {
            IsMusicMuted = isMuted;
            
            MusicMutedStateChanged?.Invoke(IsMusicMuted);
        }
    }
}