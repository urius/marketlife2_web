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
            
            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_playerCharView);
            _playerCharView = null;
        }

        private void Subscribe()
        {
        }

        private void Unsubscribe()
        {
        }
    }
}