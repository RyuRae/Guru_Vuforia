using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace FileTools
{
    public class UpdateFilesManager : MonoSingleton<UpdateFilesManager>
    {
        public class UpdateFiles
        {
            public Action onUpdated = null;
            public AssetRecordsInfo tempLocalInfo = null;
            public AssetRecordsInfo tempServerInfo = null;
            public LoadFile tempServerLoad = null;
            public List<FileInfo> localExistFileList = new List<FileInfo>(); //本地已存在的文件
            public Queue<string> redundancyQueue = new Queue<string>(); //多余文件
            public List<AssetRecord> downloadList = new List<AssetRecord>(); //只针对 ServerLoadFile 同时也表明本地不存在（因为会因版本不同会被删除后下载）
            public List<AssetRecord> checkedList = new List<AssetRecord>(); //不存在的本地文件列表
            public bool isOverwriteLocalAssetRecordsInfo = false;
            // ---------------------------------------------------------------------------------------------
            public void SetUpdateByAssetRecord(bool isLocal, AssetRecord assetRecord, bool isComparingLocalFile)
            {
                if (isComparingLocalFile)
                {
                    string fileLocalPath = FileHelper.GetFilePath(assetRecord, FileAddressType.LOCAL);
                    FileInfo localFileInfo = new FileInfo(fileLocalPath);
                    localFileInfo.Refresh();
                    if (localFileInfo.Exists)
                    {
                        long fileLastModifyTime = FileHelper.ConvertDateTimeToLong(localFileInfo.LastWriteTimeUtc);
                        if (!assetRecord.LastModifyTime.Equals(fileLastModifyTime))
                        {
                            downloadList.Add(assetRecord);
                        }
                        else
                        {
                            checkedList.Add(assetRecord);
                        }
                    }
                    else
                    {
                        downloadList.Add(assetRecord);
                    }
                }
                else
                {
                    if (isLocal)
                    {
                        checkedList.Add(assetRecord);
                    }
                    else
                    {
                        downloadList.Add(assetRecord);
                    }
                }
            }

            public void SetUpdateByComparingRecords(Dictionary<string, AssetRecord> localRecords, 
                                                    Dictionary<string, AssetRecord> serverRecords, 
                                                    bool isComparingLocalFile)
            {
                //删掉多余的资源
                var it_localRecords = localRecords.GetEnumerator();
                while (it_localRecords.MoveNext())
                {
                    if (!serverRecords.ContainsKey(it_localRecords.Current.Key))
                    {
                        AssetRecord localRecord = it_localRecords.Current.Value;
                        redundancyQueue.Enqueue(FileHelper.GetFilePath(localRecord, FileAddressType.LOCAL));
                        //Debug.Log(it_localRecords.Current.Key);
                    }
                }

                //添加更新的
                var it_serverRecords = serverRecords.GetEnumerator();
                while (it_serverRecords.MoveNext())
                {
                    if (localRecords.ContainsKey(it_serverRecords.Current.Key))
                    {
                        AssetRecord localRecord = localRecords[it_serverRecords.Current.Key];
                        AssetRecord serverRecord = it_serverRecords.Current.Value;
                        if (!localRecord.LastModifyTime.Equals(serverRecord.LastModifyTime) ||
                            !localRecord.Version.Equals(serverRecord.Version))
                        {
                            downloadList.Add(serverRecord);
                        }
                        else
                        {
                            //与本地文件进行二次校验
                            SetUpdateByAssetRecord(true, localRecord, isComparingLocalFile);
                        }
                    }
                    else
                    {
                        downloadList.Add(it_serverRecords.Current.Value);
                    }
                }
            }

            // ---------------------------------------------------------------------------------------------

            public void PrintInfo()
            {
                Debug.Log("---------------------------------PrintInfo----------------------------------");
                Debug.Log("downloadFileList --> count : " + downloadList.Count);
                var it_download = downloadList.GetEnumerator();
                while (it_download.MoveNext())
                {
                    it_download.Current.PrintInfo();
                }

                Debug.Log("checkedFileDic --> count : " + checkedList.Count);
                var it_Checked = checkedList.GetEnumerator();
                while (it_Checked.MoveNext())
                {
                    it_Checked.Current.PrintInfo();
                }

                Debug.Log("redundancyQueue --> count : " + redundancyQueue.Count);
                var it_redundancy = redundancyQueue.GetEnumerator();
                while (it_redundancy.MoveNext())
                {
                    Debug.Log(it_redundancy.Current);
                }

                Debug.Log("----------------------------------------------------------------------------");
            }
        }

        private bool isOffLine = false; //脱机模式 如果是脱机模式则不检查服务器上的资源
        private bool isComparingLocalFile = false; //与本地文件进行对比
        private bool isClearAll = false; //全部清空 然后重来
        private bool isLoadCheckedFile = false; // 下载后是否加载已经检查好的文件
        private bool isCacheBundleOnDownloading = false;
        private bool isCacheBundleOnCheckingLocalFile = false;
        private bool isAllFilesExist = true;
        ProgressMonitor monitor = new ProgressMonitor();
        // ----------------------------------------------------------------------------------------------------
        public void CheckAndUpdate(Action onUpdated)
        {
            UpdateFiles updateFiles = new UpdateFiles();
            updateFiles.onUpdated = onUpdated;

            //先加载本地的索引 也可以进行自检生成数据后 在和服务器上的索引进行对比
            updateFiles.tempLocalInfo = new AssetRecordsInfo();
            LoadFile localLoad = updateFiles.tempLocalInfo.GetLoadFile(LoadBehaviour.ContentLoadFromLoacal_WWW, (localLoaded) =>
            {
                if (isOffLine)
                {
                    SetUpdateFiles(updateFiles);
                }
                else
                {
                    updateFiles.tempServerInfo = new AssetRecordsInfo();
                    updateFiles.tempServerLoad = updateFiles.tempServerInfo.GetLoadFile(LoadBehaviour.ContentLoadFromServer_WWW, (serverLoaded) =>
                    {
                        SetUpdateFiles(updateFiles);
                    });
                    LoadFileController.Instance.Load(updateFiles.tempServerLoad);
                }
            });
            LoadFileController.Instance.Load(localLoad);
        }

        private void SetUpdateFiles(UpdateFiles updateFiles)
        {
            if (updateFiles.tempServerInfo.Version != 0)
            {
                if (updateFiles.tempLocalInfo.Version != 0)
                {
                    Debug.Log("1. 本地端与网络端索引文件均已发现，开始进行对比更新！！！");
                    //对比进行更新
                    updateFiles.isOverwriteLocalAssetRecordsInfo = !updateFiles.tempLocalInfo.Version.Equals(updateFiles.tempServerInfo.Version) ||
                                                       !updateFiles.tempLocalInfo.LastModifyTime.Equals(updateFiles.tempServerInfo.LastModifyTime);

                    if (updateFiles.isOverwriteLocalAssetRecordsInfo)
                    {
                        Debug.Log("1.1 需要更新~~~~~~");
                        FileHelper.ActionOnTraversePackingRecordType((type) =>
                        {
                            Dictionary<string, AssetRecord> localRecords = updateFiles.tempLocalInfo.GetRecordsDic(type);
                            Dictionary<string, AssetRecord> serverRecords = updateFiles.tempServerInfo.GetRecordsDic(type);
                            updateFiles.SetUpdateByComparingRecords(localRecords, serverRecords, isComparingLocalFile);
                        });
                    }
                    else
                    {
                        Debug.Log("1.2 不需要更新~~~~~~");
                    }

                    isClearAll = false;
                }
                else
                {
                    Debug.Log("2. 只发现了网络端的索引文件，进行完全更新");
                    // 全部更新 更新前情况本地资源文件夹
                    updateFiles.isOverwriteLocalAssetRecordsInfo = true;
                    updateFiles.tempServerInfo.ActionOnTraverseRecordsDic((kvp) =>
                    {
                        updateFiles.SetUpdateByAssetRecord(false, kvp.Value, isComparingLocalFile);
                    });
                    isClearAll = true;
                }
            }
            else
            {
                if (updateFiles.tempLocalInfo.Version != 0)
                {
                    Debug.Log("3. 只发现了本地端的索引文件，与本地文件映射进行检测");
                    updateFiles.isOverwriteLocalAssetRecordsInfo = false;
                    updateFiles.tempLocalInfo.ActionOnTraverseRecordsDic((kvp) =>
                    {
                        updateFiles.SetUpdateByAssetRecord(true, kvp.Value, isComparingLocalFile);
                    });
                    isClearAll = false;

                    // 索引文件与本地文件不能一一对应
                    if (updateFiles.downloadList.Count > 0)
                    {
                        Debug.Log("3.1 索引文件与本地文件不一致，无法开始体验");
                        // 严重警告界面 或断线重连界面
                        return;
                    }
                }
                else
                {
                    updateFiles.isOverwriteLocalAssetRecordsInfo = false;
                    Debug.Log("4. 网络端与本地端均未找到索引文件，无法开始体验");

                    //UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                    //UISceneHint.Instance.ShowNetworkConnectHint();
                    // 严重警告界面 或断线重连界面
                    return;
                }
            }
            //updateFiles.PrintInfo();
            ExecuteUpdate(updateFiles);
        }

        // ----------------------------------------------------------------------------------------------------
        public void ExecuteUpdate(UpdateFiles updateFiles)
        {
            //清空本地资源
            ClearAllAssetFiles();
            DownloadingOrCheckingFiles(updateFiles,()=> 
            {
                LoadFileController.Instance.ClearHttpWebRequestQueue();
                MoveFromTempToLocal(updateFiles.downloadList);
                DeleteRedundancyFiles(updateFiles.redundancyQueue, () =>
                {
                    updateFiles.downloadList.Clear();
                    updateFiles.checkedList.Clear();
                    SetAssetRecordsInfo(updateFiles);
                    SetMainManifest(updateFiles);
                });
            });
        }

        private void ClearAllAssetFiles()
        {
            if (isClearAll)
            {
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(StrConst.Instance.LocalFileAddress);
                    if (directoryInfo.Exists)
                    {
                        directoryInfo.Delete(true);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.StackTrace);
                }
            }
        }

        private void DownloadingOrCheckingFiles(UpdateFiles updateFiles, Action onAllLoaded)
        {
            monitor.State = ProgressState.DOWNLOAD;
            LoadFiles(updateFiles.downloadList, LoadBehaviour.DownloadFile_ResumeBrokenTransfer_HttpWebRequest, isCacheBundleOnDownloading, () =>
            {
                if (isLoadCheckedFile)
                {
                    monitor.State = ProgressState.CHECK;
                    LoadFiles(updateFiles.checkedList, LoadBehaviour.ContentLoadFromLoacal_WWW, isCacheBundleOnCheckingLocalFile, onAllLoaded);
                }
                else
                {
                    onAllLoaded();
                }
            });
        }

        private void LoadFiles(List<AssetRecord> list, LoadBehaviour behaviour,  bool isCacheToRecord,  Action onAllLoaded)
        {
            if (list.Count == 0 && onAllLoaded != null)
            {
                onAllLoaded();
            }
            else
            {
                int count = list.Count;
                monitor.CaculateInit(count);
                var it = list.GetEnumerator();
                while (it.MoveNext())
                {
                    AssetRecord assetRecord = it.Current;
                    LoadFile loadFile = assetRecord.GetLoadFile(behaviour, isCacheToRecord, null, (loadedFile) =>
                    {
                        if (!loadedFile.IsLoadSuccess)
                            isAllFilesExist = false;
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


        private void MoveFromTempToLocal(List<AssetRecord> list)
        {
            DirectoryInfo fromPathFolder = new DirectoryInfo(StrConst.Instance.DownloadTempAddress);
            if (!fromPathFolder.Exists)
            {
                fromPathFolder.Create();
            }

            var it = list.GetEnumerator();
            while (it.MoveNext())
            {
                AssetRecord assetRecord = it.Current;
                string fromPath = FileHelper.GetFilePath(assetRecord, FileAddressType.DOWNLOAD_TEMP);
                if (File.Exists(fromPath))
                {
                    string toPath = FileHelper.GetFilePath(assetRecord, FileAddressType.LOCAL);
                    string toFolder = Path.GetDirectoryName(toPath);
                    DirectoryInfo toFolderDir = new DirectoryInfo(toFolder);
                    if (!toFolderDir.Exists)
                    {
                        toFolderDir.Create();
                    }
                    File.Copy(fromPath, toPath, true);
                }
            }
            DeleteDirectory(fromPathFolder);
        }

        private bool DeleteDirectory(DirectoryInfo folederDirInfo)
        {
            try
            {
                FileInfo[] fileInfos = folederDirInfo.GetFiles();
                var it_fi = fileInfos.GetEnumerator();
                while (it_fi.MoveNext())
                {
                    ((FileInfo)it_fi.Current).Delete();
                }

                DirectoryInfo[] directoryInfos = folederDirInfo.GetDirectories();
                var it_di = directoryInfos.GetEnumerator();
                while (it_di.MoveNext())
                {
                    DeleteDirectory((DirectoryInfo)it_di.Current);
                }
                folederDirInfo.Delete(true);
                return true;
            }
            catch(Exception ex)
            {
                Debug.Log(ex.StackTrace);
                return false;
            }
        }

        private void DeleteRedundancyFiles(Queue<string> redundancyQueue, Action onAllDeleted)
        {
            while (redundancyQueue.Count > 0)
            {
                string filePath = redundancyQueue.Dequeue();
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.StackTrace);
                }
            }

            if (onAllDeleted != null)
            {
                onAllDeleted();
            }
        }

        private void SetAssetRecordsInfo(UpdateFiles updateFiles)
        {
            if (isAllFilesExist && updateFiles.tempServerInfo.Version != 0)
            {
                AssetsManager.Instance.RecordsInfo = updateFiles.tempServerInfo;
                //写入本地索引文件
                if (updateFiles.isOverwriteLocalAssetRecordsInfo && updateFiles.tempServerLoad != null)
                {
                    FileHelper.SaveToFile(updateFiles.tempServerLoad,
                                          updateFiles.tempServerInfo.WriteByBinaryWriter);
                }
            }
            else
            {
                AssetsManager.Instance.RecordsInfo = updateFiles.tempLocalInfo;
            }
        }

        private void SetMainManifest(UpdateFiles updateFiles)
        {
            AssetRecordsInfo info = AssetsManager.Instance.RecordsInfo;
            AssetRecord record = info.GetAssetRecord(RuntimeAssetType.MANIFEST,info.mainManifestFileName);
            if (record != null)
            {
                LoadFile loadFile = record.GetLoadFile(LoadBehaviour.ContentLoadFromLoacal_LoadBundleFile, false, (obj) =>
                {
                    AssetBundle bundle = obj as AssetBundle;
                    AssetsManager.Instance.ABManifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                }, (loaded) =>
                {
                    //string[] testArray = mainManifest.GetAllAssetBundles();
                    //var it = testArray.GetEnumerator();
                    //while (it.MoveNext())
                    //{
                    //    Debug.Log(it.Current);
                    //}
                    AssetsManager.Instance.IsInitialized = true;
                    Debug.Log("资源以及配置加载完毕！");
                    if (updateFiles.onUpdated != null)
                    {
                        updateFiles.onUpdated();
                    }
                });

                LoadFileController.Instance.Load(loadFile);
            }
        }

        // ---------------------------------------------------------------------------------------------

        //        void CreateModelFile(string path, string name, byte[] info, int length)
        //        {
        //            //文件流信息  
        //            //StreamWriter sw;  
        //            Stream sw;
        //            FileInfo t = new FileInfo(path + "//" + name);
        //            if (!t.Exists)
        //            {
        //                //如果此文件不存在则创建  
        //                sw = t.Create();
        //            }
        //            else
        //            {
        //                //如果此文件存在则打开  
        //                //sw = t.Append();  
        //                return;
        //            }
        //            //以行的形式写入信息  
        //            //sw.WriteLine(info);  
        //            sw.Write(info, 0, length);
        //            //关闭流  
        //            sw.Close();
        //            //销毁流  
        //            sw.Dispose();
        //        }

        //        /** 
        //        * path：文件创建目录 
        //        * name：文件的名称 
        //        *  info：写入的内容 
        //        */
        //        void CreateFile(string path, string name, string info)
        //        {
        //            //文件流信息  
        //            StreamWriter sw;
        //            FileInfo t = new FileInfo(path + "//" + name);
        //            if (!t.Exists)
        //            {
        //                //如果此文件不存在则创建  
        //                sw = t.CreateText();
        //            }
        //            else
        //            {
        //                //如果此文件存在则打开  
        //                sw = t.AppendText();
        //            }
        //            //以行的形式写入信息  
        //            sw.WriteLine(info);
        //            //关闭流  
        //            sw.Close();
        //            //销毁流  
        //            sw.Dispose();
        //        }

        //        /** 
        //         * 读取文本文件 
        //         * path：读取文件的路径 
        //         * name：读取文件的名称 
        //         */
        //        ArrayList LoadFile(string path, string name)
        //        {
        //            //使用流的形式读取  
        //            StreamReader sr = null;
        //            try
        //            {
        //                sr = File.OpenText(path + "//" + name);
        //            }
        //            catch (Exception e)
        //            {
        //                //路径与名称未找到文件则直接返回空  
        //                return null;
        //            }
        //            string line;
        //            ArrayList arrlist = new ArrayList();
        //            while ((line = sr.ReadLine()) != null)
        //            {
        //                //一行一行的读取  
        //                //将每一行的内容存入数组链表容器中  
        //                arrlist.Add(line);
        //            }
        //            //关闭流  
        //            sr.Close();
        //            //销毁流  
        //            sr.Dispose();
        //            //将数组链表容器返回  
        //            return arrlist;
        //        }

        //        //读取模型文件  
        //        IEnumerator LoadModelFromLocal(string path, string name)
        //        {
        //            string s = null;
        //#if UNITY_ANDROID
        //            s = "jar:file://" + path + "/" + name;
        //#elif UNITY_IPHONE
        //       s = path+"/"+name;  
        //#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
        //       s = "file://"+path+"/"+name;  
        //#endif
        //            WWW w = new WWW(s);
        //            yield return w;
        //            if (w.isDone)
        //            {
        //                Instantiate(w.assetBundle.mainAsset);
        //            }
        //        }

        // ---------------------------------------------------------------------------------------------
    }
}

