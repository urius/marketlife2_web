using UnityEngine;
using UnityEngine.Rendering;
using View.Game.Shared;

namespace View.Game.ShopObjects.Common
{
    public class ShopObjectViewBase : MonoBehaviour, ISortableView, IOwnedCellsView
    {
        [SerializeField] private SortingGroup _sortingGroup;
        
        public OwnedCellView[] OwnedCellViews { get; private set; }

        protected virtual void Awake()
        {
            OwnedCellViews = GetComponentsInChildren<OwnedCellView>();
            
            foreach (var ownedCellView in OwnedCellViews)
            {
                ownedCellView.gameObject.SetActive(false);
            }
        }

        public void SetSortingOrder(int order)
        {
            _sortingGroup.sortingOrder = order;
        }

        public void SetSortingLayerName(string sortingLayerName)
        {
            _sortingGroup.sortingLayerName = sortingLayerName;
        }
    }
}