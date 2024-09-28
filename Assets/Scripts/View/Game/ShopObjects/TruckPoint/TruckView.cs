using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using View.Game.Misc;

namespace View.Game.ShopObjects.TruckPoint
{
    public class TruckView : MonoBehaviour, ITruckBoxPositionsProvider
    {
        public const float CapAnimationDuration = 0.6f;
        
        [SerializeField] private ProductsBoxView[] _productsBoxViews;
        [SerializeField] private Transform _spritesContainer;
        [SerializeField] private Transform _truckBoxCapTransform;
        [SerializeField] private Transform _truckBoxCapOpenedPositionTransform;
        [SerializeField] private Transform _truckBoxCapClosedPositionTransform;
        [SerializeField] private Transform _truckFarPositionTransform;

        private const float TruckArriveAnimationDuration = Constants.TruckArrivingDuration - CapAnimationDuration;

        public int ProductBoxesAmount => _productsBoxViews.Length;

        public Vector3 GetBoxWorldPosition(int boxIndex)
        {
            return boxIndex < ProductBoxesAmount
                ? _productsBoxViews[boxIndex].transform.position
                : _productsBoxViews[ProductBoxesAmount - 1].transform.position;
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

        public UniTask AnimateTruckArrive()
        {
            StopCurrentTweens();
            
            _spritesContainer.position = _truckFarPositionTransform.position;

            SetCapOpenedState(false);
            
            gameObject.SetActive(true);

            var tcs = new UniTaskCompletionSource();
            
            _spritesContainer.LeanMoveLocal(Vector3.zero, TruckArriveAnimationDuration)
                .setEaseOutQuad()
                .setOnComplete(OnTruckArriveAnimationFinished)
                .setOnCompleteParam(tcs);

            return tcs.Task;
        }

        public void SetTruckArrived()
        {
            _spritesContainer.localPosition = Vector3.zero;
            SetCapOpenedState(true);
            
            gameObject.SetActive(true);
        }
        
        public void AnimateTruckMovedOut()
        {
            StopCurrentTweens();
            
            _spritesContainer.localPosition = Vector3.zero;

            AnimateCapClose();
            
            _spritesContainer.LeanMove(_truckFarPositionTransform.position, TruckArriveAnimationDuration)
                .setEaseInQuad()
                .setDelay(CapAnimationDuration)
                .setOnComplete(OnTruckMovedOutAnimationFinished);
        }

        private void OnTruckMovedOutAnimationFinished()
        {
            gameObject.SetActive(false);
        }

        public void SetTruckMovedOut()
        {
            _spritesContainer.position = _truckFarPositionTransform.position;
            
            SetCapOpenedState(false);
            
            gameObject.SetActive(false);
        }

        private void OnTruckArriveAnimationFinished(object tcs)
        {
            ((UniTaskCompletionSource)tcs).TrySetResult();
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

        private void StopCurrentTweens()
        {
            LeanTween.cancel(_spritesContainer.gameObject);
            LeanTween.cancel(_truckBoxCapTransform.gameObject);
        }
    }
}