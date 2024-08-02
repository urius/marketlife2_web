using Cysharp.Threading.Tasks;
using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using Utils;

namespace View.Game.Misc
{
    public class VFXMediator : MediatorBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        
        protected override void MediateInternal()
        {
            _eventBus.Subscribe<VFXRequestSmokeEvent>(OnVFXRequestSmokeEvent);
        }

        protected override void UnmediateInternal()
        {            
            _eventBus.Unsubscribe<VFXRequestSmokeEvent>(OnVFXRequestSmokeEvent);
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

            await smokeVFXView.PlayAsync();
            
            Destroy(smokeVFXView);
        }
    }
}