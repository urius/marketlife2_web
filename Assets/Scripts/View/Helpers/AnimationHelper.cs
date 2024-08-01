using Data;
using Holders;
using Infra.Instance;
using UnityEngine;
using View.Game.Misc;
using View.Game.People;
using View.Game.Shared;

namespace View.Helpers
{
    public static class AnimationHelper
    {
        public static TakeBoxAnimationContext InitTakeBoxFromTruckPointAnimation(ManView playerCharView, ProductsBoxView animatedBoxView, ProductType productType, Vector3 startPosition)
        {
            var spritesHolderSo = Instance.Get<SpritesHolderSo>();
            
            var productSprite = spritesHolderSo.GetProductSpriteByKey(productType);
            animatedBoxView.SetProductsSprite(productSprite);

            var transform = animatedBoxView.transform;
            transform.localEulerAngles = new Vector3(145, 45, 60);
            transform.position = startPosition;
            animatedBoxView.SetTopSortingLayer();

            playerCharView.SetProductsBoxVisibility(false);
            
            return new TakeBoxAnimationContext
            {
                StartPosition = startPosition,
                TargetTransform = playerCharView.ProductsBoxPlaceholderTransform,
                BoxView = animatedBoxView,
                ManView = playerCharView,
                Speed = 5,
            };
        }

        public static float TakeBoxAnimationUpdate(TakeBoxAnimationContext animationContext)
        {
            animationContext.Progress += Time.deltaTime * animationContext.Speed;

            animationContext.BoxView.transform.position = Vector3.Slerp(
                animationContext.StartPosition,
                animationContext.TargetTransform.position,
                animationContext.Progress);
            
            if (animationContext.Progress >= 1)
            {
                animationContext.ManView.SetProductsBoxVisibility(true);
                animationContext.Progress = 1;
            }

            return animationContext.Progress;
        }
    }
}