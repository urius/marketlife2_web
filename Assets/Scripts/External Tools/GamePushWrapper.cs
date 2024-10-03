using Cysharp.Threading.Tasks;
using Events;
using GamePush;
using Infra.EventBus;
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
        
        public static async UniTask<bool> ShowRewardedAds()
        {
#if UNITY_EDITOR
            return true;
#endif
            Dispatch(new RequestGamePauseEvent(nameof(GamePushWrapper), true));

            var showAdsResult = await Instance.ShowRewardedAdsInternal();
            
            Dispatch(new RequestGamePauseEvent(nameof(GamePushWrapper), false));

            return showAdsResult;
        }

        public static string GetLanguage()
        {
            var gpCurrentLanguage = GP_Language.Current();

            switch (gpCurrentLanguage)
            {
                case Language.Russian:
                    return "ru";
                case Language.English:
                case Language.Turkish:
                case Language.French:
                case Language.Italian:
                case Language.German:
                case Language.Spanish:
                case Language.Chineese:
                case Language.Portuguese:
                case Language.Korean:
                case Language.Japanese:
                case Language.Arab:
                case Language.Hindi:
                case Language.Indonesian:
                default:
                    return "en";
            }
        }

        private static void Dispatch(RequestGamePauseEvent e)
        {
            var eventBus = Infra.Instance.Instance.Get<IEventBus>();
            eventBus.Dispatch(e);
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
            _rewardedAdsTcs.TrySetResult(success);
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