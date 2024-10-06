using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Tools.AssetBundles
{
    [CreateAssetMenu(fileName = "AssetBundlesLoader", menuName = "ScriptableObjects/Asset Bundles/AssetBundlesLoader")]
    public class AssetBundlesLoader : ScriptableObject, IAssetBundlesLoader
    {
        public static AssetBundlesLoader Instance { get; private set; }

        [Header("AssetBundle URLs")]
        [SerializeField] private string _webGlUrl;
        [SerializeField] private string _osxUrl;
        [SerializeField] private string _androidUrl;

        private Dictionary<string, AssetBundle> _bundlesByName = new Dictionary<string, AssetBundle>();

        public AssetBundle GetLoadedBundle(string bundleName)
        {
            if (_bundlesByName.TryGetValue(bundleName, out var result))
            {
                return result;
            }

            return null;
        }

        public async UniTask<AssetBundle> LoadOrGetBundle(string bundleName, int version, Action<float> progressCallback)
        {
            if (_bundlesByName == null)
            {
                _bundlesByName = new Dictionary<string, AssetBundle>();
                Debug.Log("AssetBundlesLoader: LoadOrGetBundle -> recreated _bundlesByName");
            }

            if (_bundlesByName.TryGetValue(bundleName, out var bundle))
            {
                return bundle;
            }

            var fullUrl = $"{GetUrl()}/{bundleName}";

            AssetBundle assetBundle = null;
            
            if (fullUrl.Contains("http"))
            {
                using var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(fullUrl, (uint)version, 0);
            
                var sendRequestOperation = webRequest.SendWebRequest();
                if (progressCallback != null)
                {
                    while (!sendRequestOperation.isDone)
                    {
                        await UniTask.Delay(100, DelayType.UnscaledDeltaTime);
                        progressCallback(sendRequestOperation.progress);
                    }
                }
                var webRequestResult = await sendRequestOperation;
                assetBundle = DownloadHandlerAssetBundle.GetContent(webRequestResult);

                _bundlesByName[bundleName] = assetBundle;

                Debug.Log($"Bundle {bundleName} was loaded ");
            }
            else
            {
                assetBundle = AssetBundle.LoadFromFile(Application.dataPath + fullUrl);
            }

            return assetBundle;
        }

        private string GetUrl()
        {
            var url = Application.platform switch
            {
                RuntimePlatform.OSXEditor => _osxUrl,
                RuntimePlatform.WebGLPlayer => _webGlUrl,
                RuntimePlatform.Android => _androidUrl,
                _ => throw new Exception($"{nameof(AssetBundlesLoader)} {nameof(GetUrl)}: Unsupported platform type: {Application.platform}"),
            };

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new Exception(
                    $"{nameof(AssetBundlesLoader)} {nameof(GetUrl)}: Undefined URL for platform type: {Application.platform}");
            }

            return url;
        }

        private void OnEnable()
        {
            Instance = this;

            _bundlesByName = new Dictionary<string, AssetBundle>();

            _bundlesByName.Clear();
        }
    }
}
