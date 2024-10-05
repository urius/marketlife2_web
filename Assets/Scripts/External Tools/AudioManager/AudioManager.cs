using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Tools.AudioManager
{
    public class AudioManager : MonoBehaviour, IAudioPlayer
    {
        public static AudioManager Instance { get; private set; }

        private readonly Dictionary<int, AudioClip> _sounds = new();
        private readonly LinkedList<string> _muteRequesters = new();

        private AudioSource _musicSource;
        private AudioSource _soundsSource;
        private IAudioSettingsModel _audioSettingsModel;

        public AudioManager()
        {
            Instance = this;
        }

        public bool IsMuteRequested => _muteRequesters.Count > 0;

        private void Awake()
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            SetMusicVolume(0);
        
            _soundsSource = gameObject.AddComponent<AudioSource>();
            _soundsSource.loop = false;
            SetSoundsVolume(0);
        }

        private void Start()
        {
            UpdateAudioSettings();
        }

        private void OnDestroy()
        {
            UnsubscribeFromSettingsModel(_audioSettingsModel);
        }

        public void SetupSound(int soundId, AudioClip soundAudioClip)
        {
            _sounds.TryAdd(soundId, soundAudioClip);
        }

        public void SetSettings(IAudioSettingsModel audioSettingsModel)
        {
            UnsubscribeFromSettingsModel(_audioSettingsModel);
            
            _audioSettingsModel = audioSettingsModel;
            
            SubscribeOnSettingsModel(_audioSettingsModel);
            
            UpdateAudioSettings();
        }

        public void SetMusicVolume(float volume)
        {
            if (_musicSource != null)
            {
                _musicSource.volume = volume;
            }
        }

        public void SetSoundsVolume(float volume)
        {
            if (_soundsSource != null)
            {
                _soundsSource.volume = volume;
            }
        }
        
        public void PlaySound(int soundId)
        {
            if (_sounds.TryGetValue(soundId, out var sound))
            {
                PlaySound(sound);
            }
        }

        public void PlaySound(AudioClip sound)
        {
            _soundsSource?.PlayOneShot(sound);
        }

        public UniTask FadeInAndPlayMusicAsync(CancellationToken stopToken, AudioClip clip, float fadeInDuration = 0.5f)
        {
            if (_musicSource == null) return UniTask.CompletedTask;
            
            var registration = stopToken.Register(() => LeanTween.cancel(gameObject, callOnComplete: true));
            
            _musicSource.Stop();
            _musicSource.clip = clip;
            
            var musicFadeTsc = new UniTaskCompletionSource();
            LeanTween.value(gameObject, f => _musicSource.volume = f, _musicSource.volume, GetMusicVolume(), fadeInDuration)
                .setOnComplete(() =>
                {
                    registration.Dispose();
                    
                    musicFadeTsc.TrySetResult();
                });
            
            _musicSource.Play();

            return musicFadeTsc.Task;
        }

        public UniTask FadeOutAndStopMusicAsync(CancellationToken stopToken, float fadeOutDuration = 0.5f)
        {
            if (_musicSource == null) return UniTask.CompletedTask;
        
            var registration = stopToken.Register(() => LeanTween.cancel(gameObject, callOnComplete: true));

            var musicFadeTsc = new UniTaskCompletionSource();
            LeanTween.value(gameObject, f => _musicSource.volume = f, _musicSource.volume, 0, fadeOutDuration)
                .setOnComplete(() =>
                {
                    _musicSource.Stop();
                    registration.Dispose();
                    
                    musicFadeTsc.TrySetResult();
                });

            return musicFadeTsc.Task;
        }

        public async UniTask PlayMusicWithFade(CancellationToken stopToken, AudioClip clip, float fadeOutDuration = 0.5f, float fadeInDuration = 0.5f)
        {
            await FadeOutAndStopMusicAsync(stopToken, fadeOutDuration);
            if (stopToken.IsCancellationRequested) return;
            await FadeInAndPlayMusicAsync(stopToken, clip, fadeInDuration);
        }

        public void MuteBy(string muteRequesterId)
        {
            if (_muteRequesters.Contains(muteRequesterId)) return;
            
            _muteRequesters.AddLast(muteRequesterId);
            UpdateAudioSettings();
        }

        public void UnmuteBy(string unmuteRequesterId)
        {
            _muteRequesters.Remove(unmuteRequesterId);
            UpdateAudioSettings();
        }

        private void OnAudioMutedStateChanged(bool isAudioMuted)
        {
            UpdateAudioSettings();
        }

        private void OnMusicMutedStateChanged(bool isMusicMuted)
        {
            UpdateAudioSettings();
        }

        private void UpdateAudioSettings()
        {
            if (_audioSettingsModel != null)
            {
                SetMusicVolume(GetMusicVolume());
                SetSoundsVolume(GetSoundsVolume());
            }
        }

        private float GetMusicVolume()
        {
            if (IsMuteRequested) return 0;
            
            return _audioSettingsModel.IsAudioMuted || _audioSettingsModel.IsMusicMuted ? 0 : 0.5f;
        }

        private float GetSoundsVolume()
        {
            if (IsMuteRequested) return 0;
            
            return _audioSettingsModel.IsAudioMuted ? 0 : 0.5f;
        }

        private void SubscribeOnSettingsModel(IAudioSettingsModel audioSettingsModel)
        {
            if (audioSettingsModel == null) return;

            audioSettingsModel.AudioMutedStateChanged += OnAudioMutedStateChanged;
            audioSettingsModel.MusicMutedStateChanged += OnMusicMutedStateChanged;
        }

        private void UnsubscribeFromSettingsModel(IAudioSettingsModel audioSettingsModel)
        {
            if (audioSettingsModel == null) return;

            audioSettingsModel.AudioMutedStateChanged -= OnAudioMutedStateChanged;
            audioSettingsModel.MusicMutedStateChanged -= OnMusicMutedStateChanged;
        }
    }
}