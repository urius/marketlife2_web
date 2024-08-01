using UnityEngine;
using View.Game.Misc;
using View.Game.People;

namespace View.Game.Shared
{
    public class TakeBoxAnimationContext
    {
        public Vector3 StartPosition;
        public ProductsBoxView BoxView;
        public ManView ManView;
        public Transform TargetTransform;
        public float Progress = 0;
        public float Speed = 1;
    }
}