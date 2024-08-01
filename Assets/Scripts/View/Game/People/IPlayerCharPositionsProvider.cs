using UnityEngine;

namespace View.Game.People
{
    public interface IPlayerCharPositionsProvider
    {
        public Transform CenterPointTransform { get; }
    }
}