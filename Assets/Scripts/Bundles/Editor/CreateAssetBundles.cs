using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAssetBundles()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

    }

    [MenuItem("Assets/Clear AssetBundles")]
    static void ClearCached()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        bool success = Caching.ClearCache();
        Debug.Log("CLEAR CACHE: " + success);
    }

    [MenuItem("Assets/Unload All Bundles")]
    static void Unload() {
        AssetBundle.UnloadAllAssetBundles(true);
    }
}
