using System;
using UnityEngine;

namespace Holders
{
    public class UpdatesProvider : MonoBehaviour, IUpdatesProvider
    {
        public event Action FixedUpdateHappened;
        public event Action SecondPassed;

        private int _framesForSecondCounter;
        private int _fps;

        private void Awake()
        {
            _fps = Application.targetFrameRate;
        }

        private void FixedUpdate()
        {
            FixedUpdateHappened?.Invoke();

            _framesForSecondCounter++;
            
            if (_framesForSecondCounter >= _fps)
            {
                _framesForSecondCounter = 0;
                SecondPassed?.Invoke();
            }
        }
    }

    public interface IUpdatesProvider
    {
        public event Action FixedUpdateHappened;
        public event Action SecondPassed;
    }
}