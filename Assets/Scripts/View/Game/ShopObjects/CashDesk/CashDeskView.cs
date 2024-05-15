using UnityEngine;
using UnityEngine.Rendering;
using View.Game.Shared;
using View.Game.ShopObjects.Common;

namespace View.Game.ShopObjects.CashDesk
{
    [SelectionBase]
    public class CashDeskView : MonoBehaviour, ISortableView
    {
        [SerializeField] private SortingGroup _sortingGroup;
        
        public OwnedCellView[] OwnedCellViews { get; private set; }

        private void Awake()
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
    }
}