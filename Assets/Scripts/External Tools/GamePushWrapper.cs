using Cysharp.Threading.Tasks;
using GamePush;
using UnityEngine;

namespace Tools
{
    public class GamePushWrapper
    {
        public static readonly GamePushWrapper Instance = new GamePushWrapper();

        private readonly UniTaskCompletionSource _initTcs = new UniTaskCompletionSource();

        private UniTaskCompletionSource<bool> _rewardedAdsTcs;
        
        public static UniTask Init()
        {
            return Instance.InitInternal();
        }
        
        public static UniTask<bool> ShowRewardedAds()
        {
#if UNITY_EDITOR
            return UniTask.FromResult(true);
#endif
            return Instance.ShowRewardedAdsInternal();
        }

        public static bool CanShowRewardedAds()
        {
#if UNITY_EDITOR
            return true;
#endif
            return GP_Ads.IsRewardedAvailable();
        }

        private UniTask<bool> ShowRewardedAdsInternal()
        {
            _rewardedAdsTcs = new UniTaskCompletionSource<bool>();
            
            GP_Ads.ShowRewarded(onRewardedReward:RewardedAdsRewardedResultHandler, onRewardedClose:RewardedAdsClosedResultHandler);

            return _rewardedAdsTcs.Task;
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

        private void RewardedAdsRewardedResultHandler(string idOrTag)
        {
            _rewardedAdsTcs.TrySetResult(true);
        }
        
        private void RewardedAdsClosedResultHandler(bool success)
        {
            _rewardedAdsTcs.TrySetResult(false);
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