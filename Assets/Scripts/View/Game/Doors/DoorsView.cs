using UnityEngine;

namespace View.Game.Doors
{
    public class DoorsView : MonoBehaviour
    {
        [SerializeField] private Transform _doorLTransform;
        [SerializeField] private Transform _doorLClosePointTransform;
        [SerializeField] private Transform _doorLOpenTransform;
        [SerializeField] private Transform _doorRTransform;
        [SerializeField] private Transform _doorRClosePointTransform;
        [SerializeField] private Transform _doorROpenTransform;

        private float _doorsOpenProgress;
        
        public float ProceedOpenDoors()
        {
            if (_doorsOpenProgress >= 1) return 1;
            
            _doorsOpenProgress += Time.deltaTime * 2;

            if (_doorsOpenProgress > 1) _doorsOpenProgress = 1;

            _doorLTransform.position = Vector3.Slerp(
                _doorLClosePointTransform.position,
                _doorLOpenTransform.position,
                _doorsOpenProgress);
            
            _doorRTransform.position = Vector3.Slerp(
                _doorRClosePointTransform.position,
                _doorROpenTransform.position,
                _doorsOpenProgress);

            return _doorsOpenProgress;
        }
        
        
        public float ProceedCloseDoors()
        {
            if (_doorsOpenProgress <= 0) return 0;
            
            _doorsOpenProgress -= Time.deltaTime * 2;

            if (_doorsOpenProgress < 0) _doorsOpenProgress = 0;

            _doorLTransform.position = Vector3.Slerp(
                _doorLClosePointTransform.position,
                _doorLOpenTransform.position,
                _doorsOpenProgress);
            
            _doorRTransform.position = Vector3.Slerp(
                _doorRClosePointTransform.position,
                _doorROpenTransform.position,
                _doorsOpenProgress);

            return 1 - _doorsOpenProgress;
        }
    }
}