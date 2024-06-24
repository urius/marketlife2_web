using UnityEngine;

namespace View.Game.ShopObjects.TruckPoint
{
    public class TruckGatesView : MonoBehaviour
    {
        [SerializeField] private Transform _leftDoorTransform;
        [SerializeField] private Transform _leftDoorOpenPointTransform;
        [SerializeField] private Transform _rightDoorTransform;
        [SerializeField] private Transform _rightDoorOpenPointTransform;
        
        const float DoorAnimationDuration = 0.5f;
        
        private Vector3 _leftDoorClosedPosition;
        private Vector3 _rightDoorClosedPosition;

        private void Awake()
        {
            SetClosedPositions();
        }

        private void OnDestroy()
        {
            CancelAllTweens();
        }

        public void AnimateOpenDoors()
        {
            SetClosedPositions();

            CancelAllTweens();

            _leftDoorTransform.gameObject
                .LeanMove(_leftDoorOpenPointTransform.position, DoorAnimationDuration)
                .setEase(LeanTweenType.easeOutQuad);

            _rightDoorTransform.gameObject
                .LeanMove(_rightDoorOpenPointTransform.position, DoorAnimationDuration)
                .setEase(LeanTweenType.easeOutQuad);
        }

        public void AnimateCloseDoors()
        {
            CancelAllTweens();
            
            _leftDoorTransform.gameObject
                .LeanMove(_leftDoorClosedPosition, DoorAnimationDuration)
                .setEase(LeanTweenType.easeOutQuad);
    
            _rightDoorTransform.gameObject
                .LeanMove(_rightDoorClosedPosition, DoorAnimationDuration)
                .setEase(LeanTweenType.easeOutQuad);
        }

        private void CancelAllTweens()
        {
            LeanTween.cancel(gameObject);
        }

        private void SetClosedPositions()
        {
            _leftDoorClosedPosition = _leftDoorTransform.position;
            _rightDoorClosedPosition = _rightDoorTransform.position;
        }
    }
}