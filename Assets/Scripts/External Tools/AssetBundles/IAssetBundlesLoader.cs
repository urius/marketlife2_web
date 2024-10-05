using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Tools.AssetBundles
{
    public interface IAssetBundlesLoader
    {
        public AssetBundle GetLoadedBundle(string bundleName);
        public UniTask<AssetBundle> LoadOrGetBundle(string bundleName, int version = -1,
            Action<float> progressCallback = null);
    }
}