using FileTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FileTools
{
    public class ExecuteLoad_BundleFile : UnityExecuteLoad, ExecuteLoad
    {
        public ExecuteLoad_BundleFile(LoadFile loadFile) : base(loadFile) { }

        public IEnumerator Execute()
        {
            string loadFilePath = FileHelper.GetFilePath(loadFile.CorrelateRecord, FileAddressType.LOCAL);
            AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(loadFilePath);
            yield return createRequest;
            if (createRequest.isDone)
            {
                loadFile.IsLoadSuccess = true;
                if (loadFile.CorrelateRecord.IsBundle)
                {
                    yield return LoadBundleByRuntimeAssetType(createRequest.assetBundle);
                    //将unload释放在上一个方法中进行是不对的 可能会报错 因为两个bundle地址不同
                    createRequest.assetBundle.Unload(false);
                }
            }
        }
    }
}
