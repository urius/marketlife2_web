using Cysharp.Threading.Tasks;
using UnityEngine;
using View.Game.People;

namespace View.Game.Shared
{
    public class PlayerCharViewSharedDataHolder : IPlayerCharViewSharedDataHolder
    {
        private readonly UniTaskCompletionSource _playerCharViewSetTsc = new ();
        
        private ManView _playerCharView;

        public Vector3 PlayerCharPosition => _playerCharView.transform.position;
        public UniTask PlayerCharViewSetTask => _playerCharViewSetTsc.Task;

        public void SetView(ManView playerCharView)
        {
            _playerCharView = playerCharView;

            _playerCharViewSetTsc.TrySetResult();
        }
    }

    public interface IPlayerCharViewSharedDataHolder
    {
        Vector3 PlayerCharPosition { get; }
        UniTask PlayerCharViewSetTask { get; }
        void SetView(ManView playerCharView);
    }
}