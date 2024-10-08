namespace Tools.AudioManager
{
    public class AudioSettingsModel
    {
        public bool IsSoundsMuted { get; private set; }
        public bool IsMusicMuted { get; private set; }

        public void SetSoundsMuted(bool isMuted)
        {
            IsSoundsMuted = isMuted;
        }
        
        public void SetMusicMuted(bool isMuted)
        {
            IsMusicMuted = isMuted;
        }
    }
}