//using AIR.Util;
using LocalAsset;
using Newtonsoft.Json;
using ServerAsset;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Xml;
using UnityEngine;

public class AssetBootstrap 
{
    private bool isInit = false;
    private static AssetBootstrap _instace = null;
    private List<SaasAir> saasAir;
    public static AssetBootstrap Instace
    {
        get
        {
            if (_instace == null)
            {
                //GameObject go = new GameObject("AssetBootstrap");
                //_instace = go.AddComponent<AssetBootstrap>();

                _instace = new AssetBootstrap();

                _instace.init(() => {});
            }

            return _instace;
        }
    }

    private Action _completeFunc;
    //===================  唯一入口  ===================
    public void init(Action completeFunc, bool reInit = false)
    {
        saasAir = Driver.Instance.GetData();
        if (reInit) {
            isInit = false;
        }

        if(isInit) {
            completeFunc();
            return;
        }

        isInit = true;

        _completeFunc = completeFunc;

        bootstrapText = null;
        serverBootstrapMap.Clear();
        localBootstrapMap.Clear();
        needUpdateList.Clear();
        needLoadList.Clear();

        Action localBootstartCallBack = () =>
        {
            compareVersion();
        };

        Action serverBootstartCallBack = () =>
        {
            getLocalBootstartInfo(localBootstartCallBack);
        };

        //先去服务器获取配置文件txt
        getServerBootstartInfo(serverBootstartCallBack);

    }

    private void getServerFileList(Action callBack)
    {
        //获取ResInfo内容
        foreach (var item in saasAir)
        {
            //去掉配置文件的存储
            if (item.Name.Equals(Tips.BOOTSTRAP_NAME))
                continue;
            SaasAir info = item;
            serverFileMap.Add(info.Name, info);
        }

        callBack();
        //ServerAssetManager.Instace.getFile(Tips.SERVER_FILE_PATH, (string text) =>
        //{
        //    serverFileMap = analyzeFileInfo(text);

        //    callBack();

        //}, onError);
    }

    private void getLocalFileList()
    {
        string localFileText = LocalAssetManager.Instace.getAllText(Tips.LOCAL_RESINFO_PATH);
        //SecurityElement xml = XMLParser.LoadXML(localFileText);
        //localFileMap = analyzeFileInfo(xml);
        //Debug.LogError("getLocalFileList   " + localFileMap.Count);
    }

    private Dictionary<string, SaasAir> analyzeFileInfo(string text)
    {
        Dictionary<string, SaasAir> map = new Dictionary<string, SaasAir>();

        JsonData data = JsonConvert.DeserializeObject<JsonData>(text);
        foreach (var item in data.data)
        {
            //去掉配置文件的存储
            if (item.Name.Equals(Tips.BOOTSTRAP_NAME))
                continue;
            SaasAir info = item;
            map.Add(info.Name, info);
        }

        return map;
    }

    private Dictionary<string, SaasAir> analyzeFileInfo(SecurityElement xml)
    {
        Dictionary<string, SaasAir> map = new Dictionary<string, SaasAir>();
        foreach (SecurityElement item in xml.Children)
        {
            SaasAir info = new SaasAir(item.Attribute("id"),item.Attribute("name"),item.Attribute("remark"), item.Attribute("versionName"), item.Attribute("filePath"),
                item.Attribute("bundle"), item.Attribute("manifest"),new SaasAirType(item));

            map.Add(info.Name, info);
        }

        return null;
    }



    private void compareVersion()
    {
        if (serverBootstrapMap["app"] != localBootstrapMap["app"])
        {
            //TODO 版本不同，提示更新
            Debug.LogError("版本不同");
            return;
        }

        compareOther();

        if(needLoadList.Count == 0 && needUpdateList.Count == 0) {
            if(_completeFunc != null) {
                _completeFunc();
            }

            return;
        }

        getServerFileList(() =>
        {
            getLocalFileList();

            int needSaveFileCount = needUpdateList.Count + needLoadList.Count;
            int savedFileCount = 0;
            foreach (string updateFile in needUpdateList.Keys)
            {
                if (localFileMap.ContainsKey(updateFile))
                {
                    string localPath = localFileMap[updateFile].FilePath;
                    Debug.Log("ddddd-----      " + localPath);
                    LocalAssetManager.Instace.deleteFile(localPath);
                }

                saveFile(updateFile, (string savedFileId) =>
                {
                    savedFileCount += 1;

                    if (savedFileCount == needSaveFileCount)
                    {
                        saveFileListText();
                        saveBootstrap();

                        if (_completeFunc != null)
                        {
                            _completeFunc();
                        }
                    }
                });
            }

            foreach (string loadFile in needLoadList.Keys)
            {
                saveFile(loadFile, (string savedFileId) =>
                {
                    savedFileCount += 1;

                    if (savedFileCount == needSaveFileCount)
                    {
                        saveFileListText();
                        saveBootstrap();

                        if (_completeFunc != null)
                        {
                            _completeFunc();
                        }
                    }
                });
            }
        });
    }

    private void saveFile(string file, Action<string> callBack)
    {
        //Debug.Log("aaa===  " + file);
        string serverPath = serverFileMap[file].Bundle;
        //Debug.Log("bbb===  " + serverPath);

        ServerAssetManager.Instace.getFile(Tips.SERVER_FILE_PATH + serverPath, file, (string pathId, byte[] bytes) =>
        {
            if(bytes == null) {
                Debug.LogError("读取文件失败  :  " + pathId);
                return;
            }
            SaasAir serverInfo = serverFileMap[file];

            string localPath = Tips.LOCAL_FILE_PATH + serverInfo.FilePath;
            //Debug.Log(localPath);
            //Debug.Log(bytes.Length);
            LocalAssetManager.Instace.saveFile(localPath, bytes);


            if (localFileMap.ContainsKey(pathId))
            {
                //Debug.Log("c111cc===  " + pathId);
                //AssetFileInfo localInfo = localFileMap[pathId];
                ////localInfo.nameId = updateFile;
                //localInfo.version = serverInfo.version;
                ////localInfo.path = serverInfo.path;
                //localInfo.fileType = serverInfo.fileType;
                //localInfo.isLoad = serverInfo.isLoad;
            }
            else
            {
                //Debug.Log("c222cc===  " + pathId);
                //AssetFileInfo localInfo = new AssetFileInfo();
                //localInfo.nameId = pathId;
                //localInfo.version = serverInfo.version;
                //localInfo.path = localPath;
                //localInfo.fileType = serverInfo.fileType;
                //localInfo.isLoad = serverInfo.isLoad;

                //localFileMap.Add(pathId, localInfo);
            }

            if (callBack != null)
            {
                callBack(pathId);
            }
        });
    }

    private void saveBootstrap()
    {
        if (bootstrapText != null && bootstrapText.Length > 0)
        {
            LocalAssetManager.Instace.setText(Tips.LOCAL_BOOTSTRAP_PATH, bootstrapText);
        }

        bootstrapText = null;
    }

    private void saveFileListText()
    {
        StringBuilder buffer = new StringBuilder();
        Debug.Log("saveFileListText StringBuilder");
        //foreach (AssetFileInfo localInfo in localFileMap.Values)
        //{
        //    //Debug.Log("--- " + localInfo.getCsvText());
        //    buffer.AppendLine(localInfo.getCsvText());
        //}

        LocalAssetManager.Instace.setText(Tips.LOCAL_FILE_LIST_PATH, buffer.ToString());
    }

    private void saveFileListText(String str)
    {
        //StringBuilder buffer = new StringBuilder();
        Debug.Log("saveFileListText String");
        //foreach (AssetFileInfo localInfo in localFileMap.Values)
        //{
        //    Debug.Log("--- " + localInfo.getCsvText());
        //    buffer.AppendLine(localInfo.getCsvText());
        //}

        LocalAssetManager.Instace.setText(Tips.LOCAL_FILE_LIST_PATH, str);
    }

    private Dictionary<string, string> needUpdateList = new Dictionary<string, string>();
    private Dictionary<string, string> needLoadList = new Dictionary<string, string>();
    private Dictionary<string, SaasAir> localFileMap = null;
    private Dictionary<string, SaasAir> serverFileMap = null;

    private void compareOther()
    {
        foreach (string key in serverBootstrapMap.Keys)
        {
            string serverValue = serverBootstrapMap[key];
            if (!localBootstrapMap.ContainsKey(key))
            {
                Debug.Log("needLoadList=== " + key);
                needLoadList.Add(key, null);
                continue;
            }

            string localValue = localBootstrapMap[key];
            if (localValue != serverValue)
            {
                Debug.Log("needUpdateList=== " + key);
                needUpdateList.Add(key, null);
            }
        }
    }

    private Dictionary<string, string> serverBootstrapMap = new Dictionary<string, string>();
    private string bootstrapText = null;
    private void getServerBootstartInfo(Action callBack)
    {
        //去文件服务器下载资源
        ServerAssetManager.Instace.getFile(Tips.SERVER_FILE_PATH + getUUID(Tips.BOOTSTRAP_NAME),  (string text) =>
        {
            Debug.Log(text);
            bootstrapText = text;
            string[] confAllLine = text.Split('\r', '\n');

            foreach (string line in confAllLine)
            {
                string[] keyValue = line.Split(',');
                if(serverBootstrapMap.ContainsKey(keyValue[0]))
                {
                    Debug.Log("lalalla   " + keyValue[0]);
                    continue;
                }
                serverBootstrapMap.Add(keyValue[0], keyValue[1]);
            }

            if (callBack != null)
            {
                callBack();
            }
        },onError);
    }

    private void onError()
    {

    }

    private string getUUID(string name)
    {
        SaasAir air = null;
        if (saasAir != null)
            air = saasAir.Find(p => p.Name.Equals(name));
        return air == null ? null : air.Manifest;
    }

    //本地没有此文件，则认为此文件包含的所有文件均不存在
    //复制文件到本地
    private void initLocalBootstrapFile()
    {
        int loadedCount = 0;

        getServerFileList(() =>
        {
            //StringBuilder fileListBuffer = new StringBuilder();
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(root);
            foreach (var file in serverFileMap)
            {
                SaasAir serverFileInfo = file.Value;
                //Debug.Log(serverFileInfo.path);
                ServerAssetManager.Instace.getFile(Tips.SERVER_FILE_PATH + serverFileInfo.FilePath + serverFileInfo.Bundle, serverFileInfo.Name, (string fileName, byte[] bytes) =>
                {
                    loadedCount += 1;
                    //Debug.LogError(fileName);
                    SaasAir localFileInfo = serverFileMap[fileName];
                    //将下载的资源文件保存在本地
                    LocalAssetManager.Instace.saveFile(Tips.LOCAL_FILE_PATH + localFileInfo.FilePath, bytes);
                    localFileInfo.FilePath = Tips.LOCAL_FILE_PATH + localFileInfo.FilePath;

                    //fileListBuffer.AppendLine(localFileInfo.getCsvText());
                    XmlElement record = GetElement(xmlDoc, serverFileInfo);
                    root.AppendChild(record);
                });
            }
            saveBootstrap();
            xmlDoc.Save(Tips.LOCAL_RESINFO_PATH);
            if (_completeFunc != null)
            {
                _completeFunc();
            }
        });
    }

    private static XmlElement GetElement(XmlDocument xmlDoc, SaasAir serverFileInfo)
    {
        string id = serverFileInfo.Id;
        string name = serverFileInfo.Name;
        string remark = serverFileInfo.Remark;
        string versionName = serverFileInfo.VersionNum;
        SaasAirType airType = serverFileInfo.AirType;
        string filePath = serverFileInfo.FilePath;
        string bundle = serverFileInfo.Bundle;
        string manifest = serverFileInfo.Manifest;
        XmlElement record = xmlDoc.CreateElement("record");
        record.SetAttribute("id", id.ToString());
        record.SetAttribute("name", name);
        record.SetAttribute("remark", remark);
        record.SetAttribute("versionName", versionName);
        record.SetAttribute("filePath", filePath);
        record.SetAttribute("bundle", bundle);
        record.SetAttribute("manifest", manifest);
        XmlElement element = xmlDoc.CreateElement("airType");
        element.SetAttribute("id", airType.Id.ToString());
        element.SetAttribute("name", airType.Name);
        record.AppendChild(element);
        return record;
    }

    private Dictionary<string, string> localBootstrapMap = new Dictionary<string, string>();
    private void getLocalBootstartInfo(Action callBack)
    {
        string[] localBootstrapAllLine = LocalAssetManager.Instace.getText(Tips.LOCAL_BOOTSTRAP_PATH);

        if (localBootstrapAllLine == null)
        {
            //Debug.LogError("null  ----------------");
            //本地没有此文件，则认为此文件包含的所有文件均不存在
            //复制文件到本地

            initLocalBootstrapFile();

            return;
        }
        //Debug.LogError("getLocalBootstartInfo");
        foreach (string line in localBootstrapAllLine)
        {
            string[] keyValue = line.Split(',');
            if(keyValue.Length != 2) {
                continue;
            }

            localBootstrapMap.Add(keyValue[0], keyValue[1]);
        }

        if (callBack != null)
        {
            callBack();
        }
    }
}