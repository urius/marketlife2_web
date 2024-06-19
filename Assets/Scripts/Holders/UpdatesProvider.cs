using System;
using UnityEngine;

namespace Holders
{
    public class UpdatesProvider : MonoBehaviour, IUpdatesProvider
    {
        public event Action FixedUpdateHappened;
        public event Action SecondPassed;

        private void Start()
        {
            InvokeRepeating(nameof(InvokeSecondPassed), 0.5f, 1.0f);
        }

        private void FixedUpdate()
        {
            FixedUpdateHappened?.Invoke();
        }

        private void InvokeSecondPassed()
        {
            SecondPassed?.Invoke();
        }
    }

    public interface IUpdatesProvider
    {
        public event Action FixedUpdateHappened;
        public event Action SecondPassed;
    }
}