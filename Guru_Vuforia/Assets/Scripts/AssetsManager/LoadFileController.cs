using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace FileTools
{
    public class LoadFileController : MonoSingleton<LoadFileController>
    {
        Queue<LoadFile> localWaittingQueue = new Queue<LoadFile>();
        Dictionary<string, LoadFile> localLoadingDic = new Dictionary<string, LoadFile>();  // string = path

        Queue<LoadFile> serverWaittingQueue = new Queue<LoadFile>();
        Dictionary<string, LoadFile> serverLoadingDic = new Dictionary<string, LoadFile>(); // string = path

        //用于释放request请求
        Queue<ExecuteLoad_HttpWebRequest> httpWebRequestQueue = new Queue<ExecuteLoad_HttpWebRequest>();

        public void Load(LoadFile loadFile)
        {
            if (loadFile != null &&
                loadFile.onloadingEnd != null &&
                loadFile.CorrelateRecord.cached != null)
            {
                loadFile.onloadingEnd(loadFile.CorrelateRecord.cached);
            }
            else
            {
                CheckLoadingDic(loadFile, false);
            }
        }

        private void CheckLoadingDic(LoadFile loadFile, bool isCheckNext)
        {
            if (loadFile != null)
            {
                Queue<LoadFile> waittingQueue = null;
                Dictionary<string, LoadFile> loadingDic = null;
                byte maxCount = 0;
                string loadFilePath = loadFile.GetLoadFilePath();
                switch (loadFile.Behaviour)
                {
                    case LoadBehaviour.ContentLoadFromLoacal_WWW:
                    case LoadBehaviour.ContentLoadFromLoacal_LoadBundleFile:
                        if (!File.Exists(loadFilePath))
                        {
                            loadFile.ExecuteOnFileLoaded();
                            return;
                        }
                        else
                        {
                            waittingQueue = localWaittingQueue;
                            loadingDic = localLoadingDic;
                            maxCount = UnityExecuteLoad.MAX_EXECUTE_COUNT;
                        }
                        break;

                    case LoadBehaviour.DownloadFile_WWW:
                    case LoadBehaviour.ContentLoadFromServer_WWW:
                        waittingQueue = serverWaittingQueue;
                        loadingDic = serverLoadingDic;
                        maxCount = UnityExecuteLoad.MAX_EXECUTE_COUNT;
                        break;

                    case LoadBehaviour.DownloadFile_ResumeBrokenTransfer_HttpWebRequest:
                        waittingQueue = serverWaittingQueue;
                        loadingDic = serverLoadingDic;
                        maxCount = ExecuteLoad_HttpWebRequest.MAX_EXECUTE_COUNT;
                        break;
                }

                if (waittingQueue != null && loadingDic != null && maxCount != 0)
                {
                    LoadFile toCheckLoadFile = loadFile;
                    if (isCheckNext)
                    {
                        toCheckLoadFile = waittingQueue.Count > 0 ? waittingQueue.Dequeue() : null;
                    }

                    if (toCheckLoadFile != null)
                    {
                        if (loadingDic.Count <= maxCount)
                        {
                            loadFilePath = toCheckLoadFile.GetLoadFilePath();
                            if (!loadingDic.ContainsKey(loadFilePath))
                            {
                                loadingDic.Add(loadFilePath, toCheckLoadFile);
                                StartCoroutine(AnalyseAndExecute(toCheckLoadFile, loadingDic));
                            }
                        }
                        else
                        {
                            waittingQueue.Enqueue(toCheckLoadFile);
                        }
                    }
                }
            }
        }

        private IEnumerator AnalyseAndExecute(LoadFile loadFile, Dictionary<string, LoadFile> loadingDic)
        {
            switch (loadFile.Behaviour)
            {
                case LoadBehaviour.ContentLoadFromLoacal_LoadBundleFile:
                    yield return new ExecuteLoad_BundleFile(loadFile).Execute();
                    break;

                case LoadBehaviour.ContentLoadFromServer_WWW:
                case LoadBehaviour.ContentLoadFromLoacal_WWW:
                case LoadBehaviour.DownloadFile_WWW:
                    yield return new ExecuteLoad_WWW(loadFile).Execute();
                    break;

                case LoadBehaviour.DownloadFile_ResumeBrokenTransfer_HttpWebRequest:
                    ExecuteLoad_HttpWebRequest request = new ExecuteLoad_HttpWebRequest(loadFile);
                    httpWebRequestQueue.Enqueue(request);
                    yield return request.Execute();
                    break;
            }
            OnLoadFinish(loadFile, loadingDic);
        }

        private void OnLoadFinish(LoadFile loadFile, Dictionary<string, LoadFile> loadingDic)
        {
            string loadFilePath = loadFile.GetLoadFilePath();
            loadingDic.Remove(loadFilePath);
            loadFile.ExecuteOnFileLoaded();
            CheckLoadingDic(loadFile, true);
        }

        public void ClearHttpWebRequestQueue()
        {
            while (httpWebRequestQueue.Count > 0)
            {
                httpWebRequestQueue.Dequeue().ClearAndClose();
            }
        }

        private void OnDisable()
        {
            ClearHttpWebRequestQueue();
        }

        // -----------------------------------------------------------------------
    }
}