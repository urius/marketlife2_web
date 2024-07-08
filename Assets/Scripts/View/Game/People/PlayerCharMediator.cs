using Data;
using Infra.Instance;
using View.Game.Shared;

namespace View.Game.People
{
    public class PlayerCharMediator : MediatorBase
    {
        private readonly IPlayerCharViewSharedDataHolder _playerCharViewSharedDataHolder = Instance.Get<IPlayerCharViewSharedDataHolder>();
        
        private ManView _playerCharView;

        protected override void MediateInternal()
        {
            _playerCharView = InstantiatePrefab<ManView>(PrefabKey.Man);
            
            MediateChild<PlayerCharMovementMediator>(_playerCharView.transform);
            MediateChild(new PlayerCharMoneyAnimationMediator(_playerCharView));
            MediateChild(new PlayerCharProductsMediator(_playerCharView));

            _playerCharViewSharedDataHolder.SetView(_playerCharView);
        }

        protected override void UnmediateInternal()
        {
            Destroy(_playerCharView);
            _playerCharView = null;
        }
    }
}