using Holders;
using Infra.Instance;
using View.Game.BuildPoint;
using View.Game.Floors;
using View.Game.People;
using View.Game.ShopObjects;
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
            MediateChild<ShopObjectsMediator>(_gameRootView.ShopObjectsContainerTransform);
            MediateChild<BuildPointsMediator>(_gameRootView.ShopObjectsContainerTransform);
            MediateChild<PlayerCharMediator>(_gameRootView.PeopleContainerTransform);
        }

        protected override void UnmediateInternal()
        {
            
        }
    }
}