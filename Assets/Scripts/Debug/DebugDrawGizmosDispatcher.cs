using System;
using UnityEngine;

public class DebugDrawGizmosDispatcher : MonoBehaviour
{
    public static event Action DrawGizmosHappened;

#if UNITY_EDITOR
    private void OnEnable()
    {
        DrawGizmosHappened = null;
    }

    private void OnDisable()
    {
        DrawGizmosHappened = null;
    }

    private void OnDrawGizmos()
    {
        DrawGizmosHappened?.Invoke();
    }
#endif
}
