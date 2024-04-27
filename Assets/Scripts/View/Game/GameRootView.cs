using UnityEngine;

namespace View.Game
{
    public class GameRootView : MonoBehaviour
    {
        [SerializeField] private Transform _floorsContainerTransform;
        [SerializeField] private Transform _wallsContainerTransform;

        public Transform FloorsContainerTransform => _floorsContainerTransform;
        public Transform WallsContainerTransform => _wallsContainerTransform;
    }
}
