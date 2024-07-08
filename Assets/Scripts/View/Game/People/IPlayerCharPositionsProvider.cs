using UnityEngine;

namespace View.Game.People
{
    public interface IPlayerCharPositionsProvider
    {
        public Transform CenterPointTransform { get; }
        
        public Vector3 GetProductInBoxPosition(int productIndex);
    }
}