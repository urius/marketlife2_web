using System;

namespace Data.Dto
{
    [Serializable]
    public struct AudioSettingsDto
    {
        public bool IsAudioMuted;
        public bool IsMusicMuted;

        public AudioSettingsDto(bool isAudioMuted, bool isMusicMuted)
        {
            IsAudioMuted = isAudioMuted;
            IsMusicMuted = isMusicMuted;
        }
    }
}