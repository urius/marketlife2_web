using UnityEngine;

namespace View.Game.People
{
    public interface IPlayerCharPositionsProvider
    {
        public Transform CenterPointTransform { get; }
        public Transform RootTransform { get; }
    }
}