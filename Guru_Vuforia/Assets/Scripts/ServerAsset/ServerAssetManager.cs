using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIR.Util;

namespace ServerAsset
{
    public class ServerAssetManager : MonoBehaviour
    {
        private static ServerAssetManager _instace = null;
        public static ServerAssetManager Instace
        {
            get
            {
                if (_instace == null)
                {
                    GameObject go = new GameObject("ServerAssetManager");
                    _instace = go.AddComponent<ServerAssetManager>();
                }
                return _instace;
            }
        }

        public LinkedList<LoadAssetStruct> waitingLoad = new LinkedList<LoadAssetStruct>();
        public Dictionary<string, WWW> loadingProgress = new Dictionary<string, WWW>();
        public Dictionary<string, bool> loadFinished = new Dictionary<string, bool>();

        private void loadCompleted(LoadAssetStruct assetStruct)
        {
            if (waitingLoad.Count > 0)
                StartCoroutine(loadFile());
            WWW www = loadingProgress[assetStruct.fileId];
            loadingProgress.Remove(assetStruct.fileId);
            string error = www.error;
            if (error != null)
            {
                if (!loadFinished.ContainsKey(assetStruct.fileId))
                {
                    loadFinished.Add(assetStruct.fileId, false);
                }
            }
            else
            {
                if (!loadFinished.ContainsKey(assetStruct.fileId))
                {
                    loadFinished.Add(assetStruct.fileId, true);
                }
            }

            switch (assetStruct.fileType)
            {
                case AssetType.bootstrap:
                    break;
                case AssetType.manifest:
                    break;
                case AssetType.lesson_list:
                    break;
                case AssetType.lesson:
                    break;
                case AssetType.assetBundle:
                    break;
                case AssetType.texture2D:
                    break;
                case AssetType.text:
                    break;
                case AssetType.audioClip:
                    break;
                case AssetType.video:
                    break;
                case AssetType.scene:
                    break;
                case AssetType.bytes:
                    byte[] bytes = null;
                    if (error == null)
                    {
                        bytes = www.bytes;
                    }

                    www.Dispose();

                    Action<string, byte[]> bytekAction = assetStruct.bytekAction;
                    if (bytekAction != null)
                    {
                        bytekAction(assetStruct.fileId, bytes);
                    }
                    break;
                case AssetType.xml:
                    string xml = null;
                    if (error == null)
                        xml = www.text;
                    www.Dispose();

                    Action<string, string> stringAction = assetStruct.stringkAction;
                    if (stringAction != null)
                    {
                        stringAction(assetStruct.fileId, xml);
                    }
                    break;
                default:
                    break;
            }
        }


        public void getFile(string url, Action<string> asynResult, Action onError)
        {
            DownloadMgr.Instance.AsynDownLoadText(url, asynResult, onError);
        }

        public void getFile(string filePath, string fileId, Action<string, byte[]> func)
        {
            LoadAssetStruct assetStruct = new LoadAssetStruct();
            assetStruct.fileId = fileId;
            assetStruct.filePath = filePath;
            assetStruct.fileType = AssetType.bytes;
            assetStruct.bytekAction = func;

            //Debug.Log("111  ------ " + assetStruct.fileId);
            waitingLoad.AddLast(assetStruct);

            if (loadingProgress.Count < Tips.MAX_LOAD_COUNT)
            {
                StartCoroutine(loadFile());
            }
        }

        public void getText(string filePath, string fileId, Action<string, string> func)
        {
            LoadAssetStruct assetStruct = new LoadAssetStruct();
            assetStruct.fileId = fileId;
            assetStruct.filePath = filePath;
            assetStruct.fileType = AssetType.xml;
            assetStruct.stringkAction = func;

            waitingLoad.AddLast(assetStruct);

            if (loadingProgress.Count < Tips.MAX_LOAD_COUNT)
                StartCoroutine(loadFile());
        }

        public void getText(string filePath, Action<string> func)
        {
            string data = Driver.Instance.GetResult();
            func(data);
        }

        void AsynResult(string result)
        {
            Debug.Log(result);
            //获取到资源列表
        }

        void OnError()
        {
        }

        private IEnumerator loadFile()
        {
            LinkedListNode<LoadAssetStruct> node = waitingLoad.First;
            LoadAssetStruct assetStruct = node.Value;
            waitingLoad.RemoveFirst();
            WWW www = setLoadingFile(assetStruct.fileId, assetStruct.filePath);
            yield return www;
            loadCompleted(assetStruct);
        }

        private WWW setLoadingFile(string fileId, string filePath)
        {
            WWW www = new WWW(filePath);

            if (loadingProgress.ContainsKey(fileId))
            {
                loadingProgress[fileId] = www;
            }
            else
            {
                loadingProgress.Add(fileId, www);
            }

            return www;
        }
    }

    public class LoadAssetStruct
    {
        public string fileId { get; set; }
        public string filePath { get; set; }

        private AssetType aType;
        public AssetType fileType
        {
            get { return aType; }
            set { aType = value; }
        }

        public Action<string, byte[]> bytekAction { get; set; }
        public Action<string, string> stringkAction { get; set; }
    }

    public enum AssetType
    {
        bootstrap = 0,
        manifest = 1,
        lesson_list = 2,
        lesson = 3,
        assetBundle = 4,
        texture2D = 5,
        text = 6,
        audioClip = 7,
        video = 8,
        scene = 9,
        bytes = 10,
        xml = 11
    }
}
