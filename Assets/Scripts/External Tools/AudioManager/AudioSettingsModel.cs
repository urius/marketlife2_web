namespace Tools.AudioManager
{
    public class AudioSettingsModel
    {
        public bool IsAudioMuted { get; private set; }
        public bool IsMusicMuted { get; private set; }

        public void SetAudioMuted(bool isMuted)
        {
            IsAudioMuted = isMuted;
        }
        
        public void SetMusicMuted(bool isMuted)
        {
            IsMusicMuted = isMuted;
        }
    }
}