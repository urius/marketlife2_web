using Cysharp.Threading.Tasks;
using Events;
using Infra.EventBus;
using UnityEngine;
#if !UNITY_STANDALONE_OSX
using GamePush;
#endif

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
            Dispatch(new RequestGamePauseEvent(nameof(GamePushWrapper), isPaused:true, needMute:true));

            var showAdsResult = await Instance.ShowRewardedAdsInternal();
            
            Dispatch(new RequestGamePauseEvent(nameof(GamePushWrapper),isPaused: false));

            return showAdsResult;
        }

        public static string GetPlayerData(string fieldName)
        {
#if !UNITY_STANDALONE_OSX
            if (GP_Player.IsStub() == false)
            {
                return GP_Player.GetString(fieldName);
            }
#endif
#if UNITY_EDITOR
            return PlayerPrefs.GetString(fieldName);
#endif
            return null;
        }

        public static string GetPlayerId()
        {
#if !UNITY_STANDALONE_OSX
            return GP_Player.GetID().ToString();
#endif
            return "undefined_id";
        }

        public static void SavePlayerData(string fieldName, string value, bool needSync = true)
        {
#if !UNITY_STANDALONE_OSX
            GP_Player.Set(fieldName, value);
            
            if (needSync)
            {
                SyncPlayerData();
            }
#endif
#if UNITY_EDITOR
            PlayerPrefs.SetString(fieldName, value);
#endif
        }

        public static void SavePlayerData(string fieldName, long value, bool needSync = true)
        {
            SavePlayerData(fieldName, value.ToString(), needSync);
        }

        public static void SyncPlayerData()
        {
#if !UNITY_STANDALONE_OSX
            GP_Player.Sync();
#endif
        }

        public static void ResetPlayer()
        {
#if !UNITY_STANDALONE_OSX
            GP_Player.ResetPlayer();
#endif
#if UNITY_EDITOR
            PlayerPrefs.DeleteAll();
#endif
        }

        public static string GetLanguage()
        {
#if !UNITY_STANDALONE_OSX
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
#endif
            return "en";
        }

        private static void Dispatch(RequestGamePauseEvent e)
        {
            var eventBus = Infra.Instance.Instance.Get<IEventBus>();
            eventBus.Dispatch(e);
        }

        public static bool CanShowRewardedAds()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return GP_Ads.IsRewardedAvailable();
#endif
            return true;
        }

        private async UniTask<bool> ShowRewardedAdsInternal()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            _rewardedAdsTcs = new UniTaskCompletionSource<bool>();
            GP_Ads.ShowRewarded(onRewardedReward:RewardedAdsRewardedResultHandler, onRewardedClose:RewardedAdsClosedResultHandler);

            return await _rewardedAdsTcs.Task;
#endif
            await UniTask.Delay(500, DelayType.UnscaledDeltaTime);
            
            return true;
        }

        private UniTask InitInternal()
        {
#if !UNITY_STANDALONE_OSX
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
#endif
            return UniTask.CompletedTask;
        }

        private void OnGpInitReady()
        {
#if !UNITY_STANDALONE_OSX
            GP_Init.OnError -= OnGpInitError;
            GP_Init.OnReady -= OnGpInitReady;
#endif
            
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