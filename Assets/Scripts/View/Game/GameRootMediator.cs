using Infra.Instance;
using Providers;
using View.Game.Floors;
using View.Game.Walls;

namespace View.Game
{
    public class GameRootMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        
        private GameRootView _gameRootView;

        protected override async void MediateInternal()
        {
            _gameRootView = TargetTransform.GetComponent<GameRootView>();

            await _playerModelHolder.PlayerModelSetTask;

            MediateChild<FloorsMediator>(_gameRootView.FloorsContainerTransform);
            MediateChild<WallsMediator>(_gameRootView.WallsContainerTransform);
        }

        protected override void UnmediateInternal()
        {
            
        }
    }
}