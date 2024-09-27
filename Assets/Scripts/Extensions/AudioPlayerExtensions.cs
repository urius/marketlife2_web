using Data;
using Tools.AudioManager;

namespace Extensions
{
    public static class AudioPlayerExtensions
    {
        public static void PlaySound(this IAudioPlayer audioPlayer, SoundIdKey soundIdKey)
        {
            audioPlayer.PlaySound((int)soundIdKey);
        }
    }
}