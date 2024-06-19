using Data;
using UnityEngine;
using View.Game.Misc;

namespace View.Game.ShopObjects.TruckPoint
{
    public class TruckView : MonoBehaviour, ITruckBoxPositionsProvider
    {
        [SerializeField] private ProductsBoxView[] _productsBoxViews;
        [SerializeField] private Transform _spritesContainer;
        [SerializeField] private Transform _truckBoxCapTransform;
        [SerializeField] private Transform _truckBoxCapOpenedPositionTransform;
        [SerializeField] private Transform _truckBoxCapClosedPositionTransform;
        [SerializeField] private Transform _truckFarPositionTransform;

        private const float CapAnimationDuration = 0.5f;
        private const float TruckArriveAnimationDuration = Constants.TruckArrivingDuration - CapAnimationDuration;

        public int ProductBoxesAmount => _productsBoxViews.Length;

        public Vector3 GetBoxWorldPosition(int boxIndex)
        {
            return boxIndex < ProductBoxesAmount
                ? _productsBoxViews[boxIndex].transform.position
                : _productsBoxViews[0].transform.position;
        }

        public bool TryGetProductBoxView(int index, out ProductsBoxView productBoxView)
        {
            productBoxView = null;

            if (index >= 0 && index < _productsBoxViews.Length)
            {
                productBoxView = _productsBoxViews[index];
                return true;
            }

            return false;
        }

        public void AnimateTruckArrive()
        {
            _spritesContainer.position = _truckFarPositionTransform.position;

            SetCapOpenedState(false);
            
            gameObject.SetActive(true);
            
            _spritesContainer.LeanMoveLocal(Vector3.zero, TruckArriveAnimationDuration)
                .setEaseOutQuad()
                .setOnComplete(OnTruckArriveAnimationFinished);
        }

        public void SetTruckArrived()
        {
            _spritesContainer.localPosition = Vector3.zero;
            SetCapOpenedState(true);
            
            gameObject.SetActive(true);
        }
        
        public void SetTruckMovedOut()
        {
            _spritesContainer.position = _truckFarPositionTransform.position;
            
            SetCapOpenedState(false);
            
            gameObject.SetActive(false);
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