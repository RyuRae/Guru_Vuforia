using UnityEngine;
using FileTools;
using System;
using System.Collections.Generic;

public class AssetsManager : MonoSingleton<AssetsManager>
{
    private AssetRecordsInfo _RecordsInfo = null;
    public AssetRecordsInfo RecordsInfo
    {
        get { return _RecordsInfo; }
        set { _RecordsInfo = value; }
    }

    private AssetBundleManifest _ABManifest = null;
    public AssetBundleManifest ABManifest
    {
        get { return _ABManifest; }
        set { _ABManifest = value; }
    }

    private bool _IsInitialized = false;
    public bool IsInitialized
    {
        get { return _IsInitialized; }
        set { _IsInitialized = value; }
    }

    private void Awake()
    {
        ConsoleView consoleView = GetComponent<ConsoleView>();
        if (consoleView == null)
        {
            consoleView = gameObject.AddComponent<ConsoleView>();
        }

        DeviceInfo deviceInfo = GetComponent<DeviceInfo>();
        if (deviceInfo == null)
        {
            deviceInfo = gameObject.AddComponent<DeviceInfo>();
        }

        LoadFileController loadFileController = GetComponent<LoadFileController>();
        if (loadFileController == null)
        {
            loadFileController = gameObject.AddComponent<LoadFileController>();
        }

        UpdateFilesManager updateFilesManager = GetComponent<UpdateFilesManager>();
        if (updateFilesManager == null)
        {
            updateFilesManager = gameObject.AddComponent<UpdateFilesManager>();
        }

        DontDestroyOnLoad(gameObject);
    }

    // -----------------------------------------------------------------------------------------------------------------------------------
    public void Load(RuntimeAssetType type, string indexName,
                     bool isCacheToRecord = false, Action<object> OnAssetLoaded = null)
    {
        if (!IsInitialized)
        {
            Debug.Log("尚未初始化！");
            return;
        }

        AssetRecord record = RecordsInfo.GetAssetRecord(type, indexName);
        if (record != null)
        {
            //LoadMethod loadMethod = (type == RuntimeAssetType.BUNDLE_PREFAB ||
            //                         type == RuntimeAssetType.BUNDLE_SCENE) ?
            //                         LoadMethod.BUNDLE_FILE : LoadMethod.WWW;

            //LoadFile loadFile = record.GetLoadFile(FileAddressType.LOCAL, isCacheToRecord,
            //                                       loadMethod, FileAddressType.NULL, OnAssetLoaded);

            LoadBehaviour behaviour = (type == RuntimeAssetType.BUNDLE_PREFAB ||
                                       type == RuntimeAssetType.BUNDLE_SCENE) ?
                                       LoadBehaviour.ContentLoadFromLoacal_LoadBundleFile : 
                                       LoadBehaviour.ContentLoadFromLoacal_WWW;

            LoadFile loadFile = record.GetLoadFile(behaviour, isCacheToRecord, OnAssetLoaded);
            LoadFileController.Instance.Load(loadFile);
        }
        else
        {
            Debug.Log(indexName + "未找到此资源！！！！！");
        }
    }

    public void LoadMulti(RuntimeAssetType type, List<string> indexNames, bool isCacheToRecord,
                          LoadMethod method, Action<object> onLoadingEnd, Action onAllLoaded)
    {
        if (!IsInitialized)
        {
            Debug.Log("尚未初始化！");
            return;
        }

        Dictionary<string, AssetRecord> records = RecordsInfo.GetRecordsDic(type);
        Queue<AssetRecord> queue = new Queue<AssetRecord>();

        var it = indexNames.GetEnumerator();
        while (it.MoveNext())
        {
            if (records.ContainsKey(it.Current))
            {
                queue.Enqueue(records[it.Current]);
            }
        }
        LoadMulti(queue, isCacheToRecord, method, onLoadingEnd, onAllLoaded);
    }


    public void LoadMulti(Queue<AssetRecord> queue, bool isCacheToRecord,
                          LoadMethod method, Action<object> onLoadingEnd, Action onAllLoaded)
    {
        if (queue.Count == 0 && onAllLoaded != null)
        {
            Debug.Log("LoadMulti ~ 无任何可加载的东西 ");
            onAllLoaded();
        }
        else
        {
            int count = queue.Count;
            ProgressMonitor monitor = new ProgressMonitor(count, ProgressState.LOAD);
            while (queue.Count > 0)
            {
                AssetRecord assetRecord = queue.Dequeue();
                LoadFile loadFile = assetRecord.GetLoadFile(LoadBehaviour.ContentLoadFromLoacal_LoadBundleFile, 
                                    isCacheToRecord, onLoadingEnd, (loadedFile) =>
                    {
                        count--;
                        monitor.Refresh(loadedFile);
                        if (count == 0 && onAllLoaded != null)
                        {
                            onAllLoaded();
                        }
                    });

                LoadFileController.Instance.Load(loadFile);
            }
        }
    }

    // -----------------------------------------------------------------------------------------------------------------------------------
}
