using UnityEngine;

namespace View.Game.People
{
    public interface IManViewBoxProductsPositionsProvider
    {
        public Vector3 GetProductInBoxPosition(int productIndex);
    }
}