using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FileTools
{
    interface ExecuteLoad
    {
        IEnumerator Execute();
    }

    public class ExecuteLoadByLoadFile
    {
        protected LoadFile loadFile;

        protected ExecuteLoadByLoadFile(LoadFile loadFile)
        {
            this.loadFile = loadFile;
        }
    }

    public abstract class UnityExecuteLoad : ExecuteLoadByLoadFile
    {
        public const byte MAX_EXECUTE_COUNT = 5;

        protected UnityExecuteLoad(LoadFile loadFile) : base(loadFile)
        {
            this.loadFile = loadFile;
        }

        protected IEnumerator LoadBundleByRuntimeAssetType(AssetBundle bundle)
        {
            if (bundle != null && loadFile.CorrelateRecord != null && loadFile.onloadingEnd != null)
            {
                switch (loadFile.CorrelateRecord.Type)
                {
                    case RuntimeAssetType.BUNDLE_SCENE:
                        yield return LoadSceneAsync(bundle, loadFile.CorrelateRecord.IndexName);
                        break;

                    case RuntimeAssetType.BUNDLE_PREFAB:
                        yield return LoadObjectAsync<GameObject>(bundle, loadFile.CorrelateRecord.IndexName);
                        break;

                    case RuntimeAssetType.AUDIO:
                        yield return LoadObjectAsync<AudioClip>(bundle, loadFile.CorrelateRecord.IndexName);
                        break;

                    case RuntimeAssetType.TEXTRUE:
                        yield return LoadObjectAsync<Texture2D>(bundle, loadFile.CorrelateRecord.IndexName);
                        break;

                    case RuntimeAssetType.TEXT:
                        yield return LoadObjectAsync<TextAsset>(bundle, loadFile.CorrelateRecord.IndexName);
                        break;

                    default:
                        if (loadFile.onloadingEnd != null)
                            loadFile.onloadingEnd(bundle);
                        break;
                }
            }
        }

        protected IEnumerator LoadSceneAsync(AssetBundle bundle, string sceneName)
        {
            if (bundle.isStreamedSceneAssetBundle)
            {
                int displayProgress = 0;
                int toProgress = 0;
                //string _sceneName = System.IO.Path.GetFileNameWithoutExtension(bundle.GetAllScenePaths()[0]);
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                asyncOperation.allowSceneActivation = false;
                while (asyncOperation.progress < 0.9f)
                {
                    toProgress = (int)asyncOperation.progress * 100;
                    while (displayProgress < toProgress)
                    {
                        ++displayProgress;
                        //SetLoadingPercentage(displayProgress);
                        //yield return new WaitForEndOfFrame();
                        yield return null;
                    }
                }

                toProgress = 100;
                while (displayProgress < toProgress)
                {
                    ++displayProgress;
                    //SetLoadingPercentage(displayProgress);
                    //yield return new WaitForEndOfFrame();
                    yield return null;
                }

                asyncOperation.allowSceneActivation = true;
                yield return asyncOperation;
                if (asyncOperation.isDone)
                {
                    if (loadFile.onloadingEnd != null)
                    {
                        loadFile.onloadingEnd(null);
                    }
                }
            }
        }

        protected IEnumerator LoadObjectAsync<T>(AssetBundle bundle, string assetName)
        {
            AssetBundleRequest request = bundle.LoadAssetAsync(assetName, typeof(T));
            yield return request;
            if (request.isDone)
            {
                if (loadFile.IsCacheToRecord)
                {
                    loadFile.CorrelateRecord.cached = request.asset;
                }

                if (loadFile.onloadingEnd != null)
                {
                    loadFile.onloadingEnd(request.asset);
                }
                else
                {
                    Debug.Log(loadFile.CorrelateRecord.IndexName + " 加载失败！！！！！");
                }
            }
        }
    }
}
