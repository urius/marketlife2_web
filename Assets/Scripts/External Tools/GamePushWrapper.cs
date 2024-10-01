using Cysharp.Threading.Tasks;
using GamePush;
using UnityEngine;

namespace Tools
{
    public class GamePushWrapper
    {
        public static readonly GamePushWrapper Instance = new GamePushWrapper();

        private readonly UniTaskCompletionSource _initTcs = new UniTaskCompletionSource();

        public static UniTask Init()
        {
            return Instance.InitInternal();
        }
        
        private UniTask InitInternal()
        {
            if (GP_Init.isReady)
            {
                _initTcs.TrySetResult();
            }
            else
            {
                GP_Init.OnReady -= OnGpInitReady;
                GP_Init.OnReady += OnGpInitReady;
                GP_Init.OnError -= OnGpInitError;
                GP_Init.OnError += OnGpInitError;
            }

            return _initTcs.Task;
        }

        private void OnGpInitReady()
        {
            GP_Init.OnError -= OnGpInitError;
            GP_Init.OnReady -= OnGpInitReady;
            
            _initTcs.TrySetResult();
        }

        private void OnGpInitError()
        {
            LogError("Game Push init error!");
        }

        private void Log(string message)
        {
            Debug.Log(GetLogMessageFormat(message));
        }

        private void LogError(string message)
        {
            Debug.LogError(GetLogMessageFormat(message));
        }

        private static string GetLogMessageFormat(string message)
        {
            return $"[{nameof(GamePushWrapper)}]: {message}";
        }
    }
}