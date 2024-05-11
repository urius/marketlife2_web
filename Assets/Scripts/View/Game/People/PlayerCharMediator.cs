using Data;

namespace View.Game.People
{
    public class PlayerCharMediator : MediatorBase
    {
        private ManView _playerCharView;

        protected override void MediateInternal()
        {
            _playerCharView = InstantiatePrefab<ManView>(PrefabKey.Man);
            
            MediateChild<PlayerCharMovementMediator>(_playerCharView.transform);
            MediateChild(new PlayerCharSpendAnimationMediator(_playerCharView));
        }

        protected override void UnmediateInternal()
        {
            Destroy(_playerCharView);
            _playerCharView = null;
        }
    }
}