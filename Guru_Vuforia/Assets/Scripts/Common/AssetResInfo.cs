using AIR.Util;
using LocalAsset;
using Newtonsoft.Json;
using ServerAsset;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetResInfo {

    private bool isInit = false;
    private static AssetResInfo _instace = null;

    public static AssetResInfo Instace
    {
        get
        {
            if (_instace == null)
            {
                _instace = new AssetResInfo();

                _instace.init(() => { });
            }

            return _instace;
        }
    }

    private Action _completeFunc;
    private string resInfotxt = null;
    private Dictionary<string, string> serverResInfoMap = new Dictionary<string, string>();
    private Dictionary<string, string> localResInfoMap = new Dictionary<string, string>();
    private Dictionary<string, string> needUpdateList = new Dictionary<string, string>();
    private Dictionary<string, string> needLoadList = new Dictionary<string, string>();
    private Dictionary<string, SaasAir> localFileMap = new Dictionary<string, SaasAir>();
    private Dictionary<string, SaasAir> serverFileMap = new Dictionary<string, SaasAir>();
    //===================  唯一入口  ===================
    public void init(Action completeFunc, bool reInit = false)
    {
        if (reInit)
        {
            isInit = false;
        }

        if (isInit)
        {
            completeFunc();
            return;
        }

        isInit = true;
        _completeFunc = completeFunc;
        resInfotxt = null;
        serverResInfoMap.Clear();
        localResInfoMap.Clear();
        needUpdateList.Clear();
        needLoadList.Clear();

        Action localResInfoCallBack = () => { compareVersion(); };

        Action serverResInfoCallBack = () => { getlocalResInfo(localResInfoCallBack); };

        getServerResInfo(serverResInfoCallBack);
    }

    private void getServerResInfo(Action callBack)
    {
        ServerAssetManager.Instace.getText(Tips.SERVER_FILE_PATH + Tips.SERVER_RESINFO_PATH,"ResInfo",(string nameId, string xml) =>
        {
            resInfotxt = xml;
            var data = XMLParser.LoadXML(resInfotxt);
            
            foreach (System.Security.SecurityElement child in data.Children)
            {
                if (serverResInfoMap.ContainsKey(child.Attribute(Tips.NAME)))
                    continue;
                serverResInfoMap.Add(child.Attribute(Tips.NAME), child.Attribute(Tips.VERSION));
            }
            if (callBack != null)
                callBack();
        });

        //获取文件列表（需要从服务器读取的文件）

        ServerAssetManager.Instace.getFile(Tips.SERVER_FILE_PATH + Tips.LOCAL_FILE_LIST_PATH, (string result) =>
        {

        }, onError);

        //读取服务器的文件
        ServerAssetManager.Instace.getFile(Tips.SERVER_FILE_PATH, (string result) => {
            //返回结果反序列化
            JsonData data = JsonConvert.DeserializeObject<JsonData>(result);
            //存储serverResInfoMap
            foreach (var item in data.data)
            {
                if (serverResInfoMap.ContainsKey(item.Name))
                    continue;
                serverResInfoMap.Add(item.Name, item.VersionNum);
            }
            if (callBack != null)
                callBack();
        }, 
        onError);
    }

    //错误信息回调
    private void onError()
    {

    }

    //比较版本
    private void compareVersion()
    {

    }

    private void getlocalResInfo(Action callBack)
    {
        string localResInfotxt = LocalAssetManager.Instace.getAllText(Tips.LOCAL_RESINFO_PATH);
        if (localResInfotxt == null)
        {
            initLocalResInfoFile();
            return;
        }
    }

    private void initLocalResInfoFile()
    {
        int loadedCount = 0;
        getServerFileList(() => 
        {
            foreach (var key in serverFileMap.Keys)
            {
                if (!serverFileMap.ContainsKey(key))
                {
                    loadedCount += 1;
                    continue;
                }

                SaasAir serverFileInfo = serverFileMap[key];

            }
        });
    }

    //获取服务器资源列表
    private void getServerFileList(Action callBack)
    {
        ServerAssetManager.Instace.getText(Tips.SERVER_SAAS_PATH, (string result) => 
        {
            JsonData data = JsonConvert.DeserializeObject<JsonData>(result);
            foreach (var child in data.data)
            {
                serverFileMap.Add(child.Name, child);
            }

            callBack();
        });
    }
}
