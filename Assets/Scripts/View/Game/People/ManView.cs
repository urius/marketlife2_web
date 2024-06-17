using System;
using System.Linq;
using Other;
using UnityEngine;
using UnityEngine.Rendering;
using View.Game.Shared;

namespace View.Game.People
{
    public class ManView : MonoBehaviour, ISortableView
    {
        [SerializeField] private SortingGroup _sortingGroup;
        [SerializeField] private Animation _animation;
        [SerializeField, LabeledArray(nameof(AnimationData.AnimationKey))] private AnimationData[] _animationsData;
        
        private AnimationKey _currentAnimationKey = AnimationKey.None;

        public void SetSortingOrder(int order)
        {
            _sortingGroup.sortingOrder = order;
        }

        public void ToRightSide()
        {
            var transform1 = transform;
            
            var scale = transform1.localScale;
            scale.x = scale.z = -1;
            transform1.localScale = scale;
        }
        
        public void ToLeftSide()
        {
            var transform1 = transform;
            
            var scale = transform1.localScale;
            scale.x = scale.z = 1;
            transform1.localScale = scale;
        }
        
        public void ToIdleState()
        {
            PlayAnimationByKey(AnimationKey.Idle);
        }
        
        public void ToWalkState()
        {
            PlayAnimationByKey(AnimationKey.Walk);
        }

        private void PlayAnimationByKey(AnimationKey animationKey)
        {
            Debug.Log($"PlayAnimationByKey: {animationKey}");
            
            if (_currentAnimationKey == animationKey) return;
            
            _currentAnimationKey = animationKey;
            
            if (_animationsData.Any(a => a.AnimationKey == animationKey))
            {
                var animationData = _animationsData.FirstOrDefault(a => a.AnimationKey == animationKey);

                _animation.clip = animationData.Animation;
                _animation.Play();
            }
        }

        [Serializable]
        private struct AnimationData
        {
            public AnimationKey AnimationKey;
            public AnimationClip Animation;
        }

        private enum AnimationKey
        {
            None,
            Idle,
            Walk,
            WalkWithBox,
            IdleWithBox,
        }
    }
}