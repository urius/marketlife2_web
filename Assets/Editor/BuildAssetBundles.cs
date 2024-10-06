using UnityEditor;

public class BuildAssetBundles
{
    [MenuItem("Assets/Build Asset bundles")]
    static void BuildAll()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles/WebGL", BuildAssetBundleOptions.None, BuildTarget.WebGL);
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles/OSX", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
    }
}
