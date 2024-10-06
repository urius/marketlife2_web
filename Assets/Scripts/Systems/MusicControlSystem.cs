using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Data;
using Holders;
using Infra.Instance;
using Model;
using Model.People;
using Tools.AssetBundles;
using Tools.AudioManager;
using UnityEngine;

namespace Systems
{
    public class MusicControlSystem : ISystem
    {
        private const float MusicFadeOutDuration = 2f;
        private const float MusicFadeInDuration = 1f;

        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IAssetBundlesLoader _assetBundlesLoader = Instance.Get<IAssetBundlesLoader>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private int _currentPlayingMusicIndex = -1;
        private CustomersModel _customersModel;
        private AudioClip[] _musicList;
        private PlayerModel _playerModel;

        public async void Start()
        {
            _musicList = await LoadMusicBundle();

            await _playerModelHolder.PlayerModelSetTask;

            _playerModel = _playerModelHolder.PlayerModel;
            _customersModel = _playerModel.ShopModel.CustomersModel;

            if (_playerModel.LevelIndex > 0)
            {
                PlayMusic();
            }
            else
            {
                Subscribe();
            }
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerModel.LevelChanged += OnLevelChanged;
            //_customersModel.CustomerAdded += OnCustomerAdded;
        }

        private void Unsubscribe()
        {
            _playerModel.LevelChanged -= OnLevelChanged;
            //_customersModel.CustomerAdded -= OnCustomerAdded;
        }

        private void OnLevelChanged(int level)
        {
            _playerModel.LevelChanged -= OnLevelChanged;

            UniTask.Delay(2000)
                .ContinueWith(PlayMusic);
        }

        private void OnCustomerAdded(CustomerCharModel model)
        {
            _customersModel.CustomerAdded -= OnCustomerAdded;

            UniTask.Delay(3000)
                .ContinueWith(PlayMusic);
        }

        private UniTask PlayMusic()
        {
            return _audioPlayer.FadeInAndPlayMusicAsync(CancellationToken.None, _musicList[0], MusicFadeInDuration);
        }

        private async Task<AudioClip[]> LoadMusicBundle()
        {
            const int assetBundleUpliftMusicVersion = 6;
            
            var assetBundle =
                await _assetBundlesLoader.LoadOrGetBundle(Constants.AssetBundleUpliftMusicName, assetBundleUpliftMusicVersion);

            var musicList = assetBundle.LoadAllAssets<AudioClip>();

            Debug.Log("Music loaded: " + musicList.Length + " name: " + musicList[0].name);
            
            return musicList;
        }
    }
}