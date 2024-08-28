using Model.ShopObjects;
using UnityEngine;

namespace Events
{
    public struct ShopObjectCellsRegisteredEvent
    {
        public readonly ShopObjectModelBase ShopObjectModel;
        public readonly Vector2Int[] OwnedCells;

        public ShopObjectCellsRegisteredEvent(ShopObjectModelBase shopObjectModel, Vector2Int[] ownedCells)
        {
            ShopObjectModel = shopObjectModel;
            OwnedCells = ownedCells;
        }
    }
}