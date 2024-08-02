using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View.Game.Misc
{
    public class VFXSmokeParticleSystemView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        private UniTaskCompletionSource _playTcs;
        
        public UniTask PlayAsync()
        {
            _particleSystem.Play();
            
            _playTcs = new UniTaskCompletionSource();

            return _playTcs.Task;
        }
        
        private void OnParticleSystemStopped()
        {
            _playTcs?.TrySetResult();
        }
    }
}