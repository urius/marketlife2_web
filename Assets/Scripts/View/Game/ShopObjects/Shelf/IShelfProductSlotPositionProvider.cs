using UnityEngine;

namespace View.Game.ShopObjects.Shelf
{
    public interface IShelfProductSlotPositionProvider
    {
        public Vector3 GetSlotWorldPosition(int slotIndex);
    }
}