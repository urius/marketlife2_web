using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Holders;
using Infra.Instance;
using Tools.AudioManager;
using Tools.StreamingAudioPlayer;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems
{
    public class MusicControlSystem : ISystem
    {
        private const float MusicFadeOutDuration = 2f;
        private const float MusicFadeInDuration = 2f;
        
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IStreamingAudioLoader _streamingAudioLoader = Instance.Get<IStreamingAudioLoader>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();

        private const string InitialThemeURL = "https://s3.eponesh.com/games/files/17247/init.mp3";
        private const string UpliftedThemeURL = "https://s3.eponesh.com/games/files/17247/uplift.mp3";
        private const string BuildThemeURL = "https://s3.eponesh.com/games/files/17247/build.mp3";

        private readonly string[] _musicUrls = { InitialThemeURL, UpliftedThemeURL, BuildThemeURL };

        private int _currentPlayingMusicIndex = -1;
        
        public async void Start()
        {
            var musicUrl = InitialThemeURL;
            _currentPlayingMusicIndex = Array.IndexOf(_musicUrls, musicUrl);
            
            var audioClip = await LoadStreamingAudioClip(musicUrl);

            await _playerModelHolder.PlayerModelSetTask;

            FadeInAndPlayMusic(audioClip);

            await WaitForAudioClipAlmostFinished(audioClip);
            
            PlayNextRandomMusic().Forget();
        }

        private static UniTask WaitForAudioClipAlmostFinished(AudioClip audioClip)
        {
            if (audioClip == null || audioClip.length <= 0)
            {
                return UniTask.Delay(1000);
            }

            return UniTask.Delay((int)((audioClip.length - MusicFadeOutDuration) * 1000));
        }

        private async UniTaskVoid PlayNextRandomMusic()
        {
            var safeCounter = 100;
            var nextMusicIndex = _currentPlayingMusicIndex;
            
            while (nextMusicIndex == _currentPlayingMusicIndex 
                   && safeCounter > 0)
            {
                safeCounter--;
                nextMusicIndex = Random.Range(0, _musicUrls.Length);
            }

            var nextMusicUrl = _musicUrls[nextMusicIndex];
            _currentPlayingMusicIndex = Array.IndexOf(_musicUrls, nextMusicUrl);

            var fadeOutTask = _audioPlayer.FadeOutAndStopMusicAsync(CancellationToken.None, MusicFadeOutDuration);
            
            var audioClip = await LoadStreamingAudioClip(nextMusicUrl);

            await fadeOutTask;
            
            FadeInAndPlayMusic(audioClip);
            
            await WaitForAudioClipAlmostFinished(audioClip);
            
            PlayNextRandomMusic().Forget();
        }

        private void FadeInAndPlayMusic(AudioClip audioClip)
        {
            _audioPlayer.FadeInAndPlayMusicAsync(CancellationToken.None, audioClip, MusicFadeInDuration);
        }

        private UniTask<AudioClip> LoadStreamingAudioClip(string musicUrl)
        {
            return _streamingAudioLoader.GetStreamingAudioClip(musicUrl, AudioType.MPEG);
        }

        public void Stop()
        {
        }
    }
}