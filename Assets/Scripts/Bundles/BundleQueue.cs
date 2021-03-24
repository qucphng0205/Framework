using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleQueueItem {
    public BundleInformation bundleInformation;
    public Action onFailed;
    public Action<AssetBundle> onSuccess;

    public BundleQueueItem(BundleInformation bundleInformation, Action<AssetBundle> onSuccess, Action onFailed = null) {
        this.bundleInformation = bundleInformation;
        this.onSuccess = onSuccess;
        this.onFailed = onFailed;
    }
}

//for loading multiple bundles
public class BundleQueue : MonoSingleton<BundleQueue>
{
    BundleLoader bundleLoader;
    Queue<BundleQueueItem> bundleQueue;
    BundleQueueItem current;

    bool queueIsBusy;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
            bundleQueue = new Queue<BundleQueueItem>();
            bundleLoader = gameObject.GetOrAddComponent<BundleLoader>();
#if UNITY_EDITOR
            //bundleLoader.isUsingLocalBundle = true;
#endif
        }
    }

    private void OnEnable()
    {
        this.RegisterListener(EventID.LoadingSuccessful, OnLoadingSuccessful);
        this.RegisterListener(EventID.LoadingFailed, OnLoadingFailed);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.LoadingSuccessful, OnLoadingSuccessful);
        this.RemoveListener(EventID.LoadingFailed, OnLoadingFailed);
    }

    public void QueueMe(BundleInformation bundleInformation, Action<AssetBundle> onSuccess, Action onFailed = null) {
        bundleQueue.Enqueue(new BundleQueueItem(bundleInformation, onSuccess, onFailed));
        StartCoroutine(LoadingQueue());
    }

    IEnumerator LoadingQueue() {
        if (queueIsBusy)
            yield break;
        queueIsBusy = true;

        while (bundleQueue.Count > 0)
        {
            while (bundleLoader.IsLoading)
                yield return null;

            current = bundleQueue.Dequeue();
            bundleLoader.LoadBundle(current.bundleInformation);
        }

        queueIsBusy = false;
    }

    void OnLoadingSuccessful(object obj)
    {
        AssetBundle assetBundle = obj as AssetBundle;
        if (current != null) {
            current.onSuccess?.Invoke(assetBundle);
        }
    }

    void OnLoadingFailed(object obj)
    {
        if (current != null)
        {
            current.onFailed?.Invoke();
        }
    }


}
