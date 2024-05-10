using Cysharp.Threading.Tasks;
using Model;

namespace Holders
{
    public class PlayerModelHolder : IPlayerModelHolder
    {
        private readonly UniTaskCompletionSource _playerModelSetTcs = new UniTaskCompletionSource();
        
        public PlayerModel PlayerModel { get; private set; }

        public UniTask PlayerModelSetTask => _playerModelSetTcs.Task;

        public void SetModel(PlayerModel model)
        {
            PlayerModel = model;
            
            _playerModelSetTcs.TrySetResult();
        }
    }

    public interface IPlayerModelHolder
    {
        public PlayerModel PlayerModel { get; }
        public UniTask PlayerModelSetTask { get; }
    }
}