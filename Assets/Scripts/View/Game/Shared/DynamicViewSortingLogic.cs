using Holders;
using UnityEngine;
using View.Helpers;

namespace View.Game.Shared
{
    public class DynamicViewSortingLogic
    {
        private readonly ISortableView _view;
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder;

        public DynamicViewSortingLogic(ISortableView view, IOwnedCellsDataHolder ownedCellsDataHolder)
        {
            _view = view;
            _ownedCellsDataHolder = ownedCellsDataHolder;
        }

        public void UpdateSorting(Vector2Int cellCoords)
        {
            foreach (var nearOffset in SortingOrderHelper.NearOffsets4)
            {
                var cellToCheck = cellCoords + nearOffset;
                if (_ownedCellsDataHolder.TryGetShopObjectOwner(cellToCheck, out var shopObjectOwnerData))
                {
                    var sortingOrder =
                        SortingOrderHelper.GetSortingOrderNearShopObject(shopObjectOwnerData.OwnedCells, cellCoords); 
                    _view.SetSortingOrder(sortingOrder);
                    
                    return;
                }
            }
            
            _view.SetSortingOrder(SortingOrderHelper.GetDefaultSortingOrderByCoords(cellCoords));
        }
    }
}