using Cysharp.Threading.Tasks;
using Data;
using Events;
using Extensions;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Tools.AudioManager;
using Utils;

namespace View.Game.Misc
{
    public class FXMediator : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();

        private bool _blockAudioFlag = false;
        private PlayerModel _playerModel;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            
            _eventBus.Subscribe<VFXRequestSmokeEvent>(OnVFXRequestSmokeEvent);
            _playerModel.InsufficientFunds += OnInsufficientFunds;
        }

        protected override void UnmediateInternal()
        {            
            _eventBus.Unsubscribe<VFXRequestSmokeEvent>(OnVFXRequestSmokeEvent);
            _playerModel.InsufficientFunds -= OnInsufficientFunds;
        }

        private void OnInsufficientFunds(int _)
        {
            _audioPlayer.PlaySound(SoundIdKey.InsufficientFunds);
        }

        private void OnVFXRequestSmokeEvent(VFXRequestSmokeEvent e)
        {
            ShowSmokeVFX(e).Forget();
        }

        private async UniTaskVoid ShowSmokeVFX(VFXRequestSmokeEvent e)
        {
            var smokeVFXView =
                InstantiatePrefab<VFXSmokeParticleSystemView>(
                    e.UseBigSmoke ? PrefabKey.VFXBigSmoke : PrefabKey.VFXSmoke);

            smokeVFXView.transform.position = _gridCalculator.GetCellCenterWorld(e.CellPosition);

            if (_blockAudioFlag == false)
            {
                _blockAudioFlag = true;
                _audioPlayer.PlaySound(SoundIdKey.Puff);
            }

            await smokeVFXView.PlayAsync();

            _blockAudioFlag = false;
            
            Destroy(smokeVFXView);
        }
    }
}