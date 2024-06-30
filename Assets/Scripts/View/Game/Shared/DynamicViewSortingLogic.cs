using Holders;
using UnityEngine;
using View.Helpers;

namespace View.Game.Shared
{
    public static class DynamicViewSortingLogic
    {
        private const string SortingLayerGeneralName = "General";
        private const string SortingLayerPeopleOutOfShopName = "PeopleOusideShop";
        
        
        public static void UpdateSorting(
            ISortableView view, IOwnedCellsDataHolder ownedCellsDataHolder, Vector2Int cellCoords)
        {
            foreach (var nearOffset in SortingOrderHelper.NearOffsets4)
            {
                var cellToCheck = cellCoords + nearOffset;
                if (ownedCellsDataHolder.TryGetShopObjectOwner(cellToCheck, out var shopObjectOwnerData))
                {
                    var sortingOrder =
                        SortingOrderHelper.GetSortingOrderNearShopObject(shopObjectOwnerData.OwnedCells, cellCoords);
                    view.SetSortingOrder(sortingOrder);

                    return;
                }
            }

            view.SetSortingLayerName(cellCoords.y < 0 ? SortingLayerPeopleOutOfShopName : SortingLayerGeneralName);
            view.SetSortingOrder(SortingOrderHelper.GetDefaultSortingOrderByCoords(cellCoords));
        }
    }
}