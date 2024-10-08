using System;

namespace Data.Dto
{
    [Serializable]
    public struct AudioSettingsDto
    {
        public bool IsSoundsMuted;
        public bool IsMusicMuted;

        public AudioSettingsDto(bool isSoundsMuted, bool isMusicMuted)
        {
            IsSoundsMuted = isSoundsMuted;
            IsMusicMuted = isMusicMuted;
        }
    }
}