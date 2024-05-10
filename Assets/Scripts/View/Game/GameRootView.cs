using UnityEngine;

namespace View.Game
{
    public class GameRootView : MonoBehaviour
    {
        [SerializeField] private Transform _floorsContainerTransform;
        [SerializeField] private Transform _wallsContainerTransform;
        [SerializeField] private Transform _peopleContainerTransform;
        [SerializeField] private Transform _shopObjectsContainerTransform;

        public Transform FloorsContainerTransform => _floorsContainerTransform;
        public Transform WallsContainerTransform => _wallsContainerTransform;
        public Transform PeopleContainerTransform => _peopleContainerTransform;
        public Transform ShopObjectsContainerTransform => _shopObjectsContainerTransform;
    }
}
