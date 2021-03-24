using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public struct BundleInformation
{
    public string url;
    public string name;
    public uint crc;
    public uint version;

    public BundleInformation(string url, string name, uint version = 1, uint crc = 0)
    {
        this.url = url;
        this.name = name;
        this.version = version;
        this.crc = crc;
    }
}

public class BundleLoader : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("DEBUG MODE")]
    public bool isUsingLocalBundle = false;
#endif

    Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
    bool isLoading;
    public bool IsLoading { get => isLoading; }

    private void OnDisable()
    {
        foreach (var item in bundles)
        {
            if (item.Value != null)
                item.Value.Unload(true);
        }
        AssetBundle.UnloadAllAssetBundles(true);
    }

    public void LoadBundle(BundleInformation bundle)
    {

#if UNITY_EDITOR
        if (isUsingLocalBundle)
        {
            if (bundles.ContainsKey(bundle.name) && bundles[bundle.name] != null)
                this.PostEvent(EventID.LoadingSuccessful, bundles[bundle.name]);
            else
                StartCoroutine(CR_LoadAssetsFromFileAsync(bundle.name));
            return;
        }
#endif
        if (bundles.ContainsKey(bundle.name) && bundles[bundle.name] != null)
            this.PostEvent(EventID.LoadingSuccessful, bundles[bundle.name]);
        else
        {
            if (bundles.ContainsKey(bundle.name))
                bundles.Remove(bundle.name);
            StartCoroutine(CR_LoadBundleFromServer(bundle.url, bundle.name, bundle.version, bundle.crc));
        }
    }

    IEnumerator CR_LoadBundleFromServer(string url, string abName, uint version, uint crc)
    {
        isLoading = true;

        while (!Caching.ready)
            yield return null;

        Debug.Log("BundleManager: Start loading assets from server: " + url + abName);
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url + abName, version, crc))
        {
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();

            //notify listener about progress of loading bundle process
            while (!operation.isDone)
            {
                this.PostEvent(EventID.LoadingProgress, operation.progress * 0.9f);
                yield return null;
            }

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                this.PostEvent(EventID.LoadingFailed, request.error);
            }
            else
            {
                AssetBundle remoteAssetBundle = DownloadHandlerAssetBundle.GetContent(request);

                if (!remoteAssetBundle)
                {
                    isLoading = false;
                    this.PostEvent(EventID.LoadingFailed, "Failed to extract AssetBundle!");
                }
                else
                {
                    if (remoteAssetBundle != null)
                        bundles[abName] = remoteAssetBundle;

                    //SIMULATION-------------------------------------
                    float x = 0.9f;
                    while (x <= 1.0f)
                    {
                        this.PostEvent(EventID.LoadingProgress, x);
                        yield return null;
                        x += 0.001f;
                    }
                    this.PostEvent(EventID.LoadingSuccessful, remoteAssetBundle);
                    //------------------------------------------------
                }
            }
        }
        isLoading = false;
    }

    //Load asset from project folder (for testing purpose)
    IEnumerator CR_LoadAssetsFromFileAsync(string abName)
    {
        AssetBundleCreateRequest asyncBundleRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, abName));

        //waiting for async bundle request loaded
        while (!asyncBundleRequest.isDone)
        {
            this.PostEvent(EventID.LoadingProgress, asyncBundleRequest.progress * 0.9f);
            yield return null;
        }

        AssetBundle localAssetBundle = asyncBundleRequest.assetBundle;

        if (localAssetBundle == null)
        {
            this.PostEvent(EventID.LoadingFailed, "Failed to extract AssetBundle!");
            yield break;
        }
        else
        {
            if (localAssetBundle != null)
                bundles[abName] = localAssetBundle;
        }

        //SIMULATION---------
        float x = 0.9f;
        while (x <= 1.0f)
        {
            this.PostEvent(EventID.LoadingProgress, x);
            yield return null;
            x += 0.001f;
        }

        this.PostEvent(EventID.LoadingSuccessful, localAssetBundle);
        //--------------------
    }
}
