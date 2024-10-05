using System.Threading;
using System.Threading.Tasks;
using Data;
using Holders;
using Infra.Instance;
using Model.People;
using Tools.AssetBundles;
using Tools.AudioManager;
using UnityEngine;

namespace Systems
{
    public class MusicControlSystem : ISystem
    {
        private const int AssetBundleUpliftMusicVersion = 1;
        private const float MusicFadeOutDuration = 2f;
        private const float MusicFadeInDuration = 2f;

        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IAssetBundlesLoader _assetBundlesLoader = Instance.Get<IAssetBundlesLoader>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private int _currentPlayingMusicIndex = -1;
        private CustomersModel _customersModel;
        private AudioClip[] _musicList;

        public async void Start()
        {
            _musicList = await LoadMusicBundle();

            await _playerModelHolder.PlayerModelSetTask;

            _customersModel = _playerModelHolder.PlayerModel.ShopModel.CustomersModel;

            Subscribe();
        }

        public void Stop()
        {
            _customersModel.CustomerAdded -= OnCustomerAdded;
        }

        private void Subscribe()
        {
            _customersModel.CustomerAdded += OnCustomerAdded;
        }

        private void OnCustomerAdded(CustomerCharModel obj)
        {
            _customersModel.CustomerAdded -= OnCustomerAdded;

            _audioPlayer.FadeInAndPlayMusicAsync(CancellationToken.None, _musicList[0], MusicFadeInDuration);
        }

        private async Task<AudioClip[]> LoadMusicBundle()
        {
            var assetBundle =
                await _assetBundlesLoader.LoadOrGetBundle(Constants.AssetBundleUpliftMusicName, AssetBundleUpliftMusicVersion);

            var upliftMusic = assetBundle.LoadAllAssets<AudioClip>();

            Debug.Log("Music loaded: " + upliftMusic.Length + " name: " + upliftMusic[0].name);
            return upliftMusic;
        }
    }
}