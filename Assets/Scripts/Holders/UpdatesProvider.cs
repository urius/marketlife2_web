using System;
using UnityEngine;

namespace Holders
{
    public class UpdatesProvider : MonoBehaviour, IUpdatesProvider
    {
        public event Action FixedUpdateHappened;
        
        private void FixedUpdate()
        {
            FixedUpdateHappened?.Invoke();
        }
    }

    public interface IUpdatesProvider
    {
        public event Action FixedUpdateHappened;
    }
}