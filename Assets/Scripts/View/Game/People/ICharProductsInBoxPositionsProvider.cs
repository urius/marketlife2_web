using UnityEngine;

namespace View.Game.People
{
    public interface ICharProductsInBoxPositionsProvider
    {
        public Vector3 GetProductInBoxPosition(int productIndex);
    }
}