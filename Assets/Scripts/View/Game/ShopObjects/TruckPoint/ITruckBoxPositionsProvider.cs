using UnityEngine;

namespace View.Game.ShopObjects.TruckPoint
{
    public interface ITruckBoxPositionsProvider
    {
        public Vector3 GetBoxWorldPosition(int boxIndex);
    }
}