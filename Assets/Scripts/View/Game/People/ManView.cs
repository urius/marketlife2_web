using UnityEngine;
using UnityEngine.Rendering;
using View.Game.Shared;

namespace View.Game.People
{
    public class ManView : MonoBehaviour, ISortableView
    {
        [SerializeField] private SortingGroup _sortingGroup;

        public void SetSortingOrder(int order)
        {
            _sortingGroup.sortingOrder = order;
        }
    }
}