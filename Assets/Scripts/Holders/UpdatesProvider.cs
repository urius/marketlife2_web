using System;
using System.Collections;
using UnityEngine;

namespace Holders
{
    public class UpdatesProvider : MonoBehaviour, IUpdatesProvider
    {
        public event Action RealtimeUpdate;
        public event Action GameplayFixedUpdate;
        public event Action RealtimeSecondPassed;
        public event Action GameplaySecondPassed;
        public event Action GameplayHalfSecondPassed;
        public event Action GameplayQuarterSecondPassed;

        private int _quarterInvokeCount = 0;

        private void Start()
        {
            InvokeRepeating(nameof(InvokeQuarterSecondPassed), 0.5f, 0.25f);

            StartCoroutine(nameof(DispatchRealtimeSecondCoroutine));
        }

        private void OnDestroy()
        {
            StopCoroutine(nameof(DispatchRealtimeSecondCoroutine));
        }

        private void Update()
        {
            RealtimeUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            GameplayFixedUpdate?.Invoke();
        }

        private void InvokeQuarterSecondPassed()
        {
            GameplayQuarterSecondPassed?.Invoke();

            _quarterInvokeCount++;
            
            if (_quarterInvokeCount % 2 == 0)
            {
                GameplayHalfSecondPassed?.Invoke();
            }
            
            if (_quarterInvokeCount % 4 == 0)
            {
                GameplaySecondPassed?.Invoke();
                
                _quarterInvokeCount = 0;
            }
        }

        private IEnumerator DispatchRealtimeSecondCoroutine()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);

                RealtimeSecondPassed?.Invoke();
            }
        }
    }

    public interface IUpdatesProvider
    {
        public event Action RealtimeUpdate;
        public event Action GameplayFixedUpdate;
        public event Action RealtimeSecondPassed;
        public event Action GameplaySecondPassed;
        public event Action GameplayHalfSecondPassed;
        public event Action GameplayQuarterSecondPassed;
    }
}