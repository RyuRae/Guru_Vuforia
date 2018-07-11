using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileTools;
using System;

namespace FileTools
{
    public class ExecuteLoad_WWW : UnityExecuteLoad, ExecuteLoad
    {
        public ExecuteLoad_WWW(LoadFile loadFile) : base(loadFile) { }

        public IEnumerator Execute()
        {
            string loadFilePath = loadFile.GetLoadFilePath();
            loadFilePath = loadFile.LoadPathType == FileAddressType.LOCAL ?
                           StrConst.FILE_PREFIX + loadFilePath :
                           loadFilePath;
            string uriStr = Uri.EscapeUriString(loadFilePath);
            //因为有中文 或者 空格所以要用 Uri.EscapeUriString
            using (WWW www = new WWW(uriStr))
            {
                #region 超时设置
                float timer = 0;
                float timeOut = 10;
                bool failed = false;

                while (!www.isDone)
                {
                    if (timer > timeOut) { failed = true; break; }
                    timer += Time.deltaTime;
                    yield return null;
                }
                if (failed || !string.IsNullOrEmpty(www.error))
                {
                    www.Dispose();
                    yield break;
                }
                #endregion

                yield return www;
                if (www.error != null)
                {
                    Debug.Log("error is " + www.error + "path : " + uriStr);
                    ManagerEvent.Send(ManagerEvent.MSG_ServerConnection, www.error);
                    loadFile.IsLoadSuccess = false;
                }
                else
                {
                    if (www.isDone)
                    {
                        loadFile.IsLoadSuccess = true;
                        FileHelper.SaveToFile(loadFile, www.bytes);
                        if (loadFile.CorrelateRecord.IsBundle)
                        {
                            yield return LoadBundleByRuntimeAssetType(www.assetBundle);
                            // 将unload释放在上一个方法中进行是不对的 可能会报错 因为两个bundle地址不同
                            www.assetBundle.Unload(false);
                        }
                        else
                        {
                            LoadWWWRuntimeAssetType(www);
                        }
                    }
                }
            }
        }

        public void LoadWWWRuntimeAssetType(WWW www)
        {
            if (www != null && loadFile.CorrelateRecord != null && loadFile.onloadingEnd != null)
            {
                object obj = www;
                switch (loadFile.CorrelateRecord.Type)
                {
                    case RuntimeAssetType.TEXT: obj = www.text; break;
                    case RuntimeAssetType.TEXTRUE: obj = www.texture; break;
                    case RuntimeAssetType.AUDIO: obj = www.GetAudioClip(); break;
                }

                if (loadFile.IsCacheToRecord)
                {
                    loadFile.CorrelateRecord.cached = obj;
                }

                if (loadFile.onloadingEnd != null)
                {
                    loadFile.onloadingEnd(obj);
                }
            }
        }

    }
}
