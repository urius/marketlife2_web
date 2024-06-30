using System;
using UnityEngine;

namespace Holders
{
    public class UpdatesProvider : MonoBehaviour, IUpdatesProvider
    {
        public event Action GameplayFixedUpdate;
        public event Action SecondPassed;
        public event Action HalfSecondPassed;
        public event Action QuarterSecondPassed;

        private int _quarterInvokeCount = 0;

        private void Start()
        {
            InvokeRepeating(nameof(InvokeQuarterSecondPassed), 0.5f, 0.25f);
        }

        private void FixedUpdate()
        {
            GameplayFixedUpdate?.Invoke();
        }

        private void InvokeQuarterSecondPassed()
        {
            QuarterSecondPassed?.Invoke();

            _quarterInvokeCount++;
            
            if (_quarterInvokeCount % 2 == 0)
            {
                HalfSecondPassed?.Invoke();
            }
            
            if (_quarterInvokeCount % 4 == 0)
            {
                SecondPassed?.Invoke();
                
                _quarterInvokeCount = 0;
            }
        }
    }

    public interface IUpdatesProvider
    {
        public event Action GameplayFixedUpdate;
        public event Action SecondPassed;
        public event Action HalfSecondPassed;
        public event Action QuarterSecondPassed;
    }
}