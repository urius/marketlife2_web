using UnityEngine;
using View.Game.Misc;

namespace View.Game.ShopObjects.TruckPoint
{
    public class TruckView : MonoBehaviour
    {
        [SerializeField] private ProductsBoxView[] _productsBoxViews;
        [SerializeField] private Transform _spritesContainer;
        [SerializeField] private Transform _truckBoxCapTransform;
        [SerializeField] private Transform _truckBoxCapOpenedPositionTransform;
        [SerializeField] private Transform _truckBoxCapClosedPositionTransform;
        [SerializeField] private Transform _truckFarPositionTransform;

        private const float CapAnimationDuration = 0.5f;
        private const float TruckArriveAnimationDuration = 1.5f;
        
        public ProductsBoxView GetProductBoxView(int index)
        {
            return _productsBoxViews[index];
        }

        public void AnimateTruckArrive()
        {
            _spritesContainer.position = _truckFarPositionTransform.position;

            SetCapOpenedState(false);
            
            _spritesContainer.LeanMoveLocal(Vector3.zero, TruckArriveAnimationDuration)
                .setEaseOutQuad()
                .setOnComplete(OnTruckArriveAnimationFinished);
        }

        private void OnTruckArriveAnimationFinished()
        {
            AnimateCapOpen();
        }

        public void AnimateCapOpen()
        {
            _truckBoxCapTransform.LeanMove(_truckBoxCapOpenedPositionTransform.position, CapAnimationDuration);
        }

        public void AnimateCapClose()
        {
            _truckBoxCapTransform.LeanMove(_truckBoxCapClosedPositionTransform.position, CapAnimationDuration);
        }

        private void SetCapOpenedState(bool isOpened)
        {
            _truckBoxCapTransform.position = isOpened
                ? _truckBoxCapOpenedPositionTransform.position
                : _truckBoxCapClosedPositionTransform.position;
        }
    }
}