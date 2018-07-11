using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AIR.Util;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System;
using UnityEngine.SceneManagement;
using Util;

public class Driver : MonoBehaviour {

    public static Driver Instance;
    //DownloadMgr download;
    readonly string fileName = "/ResInfo.xml";
    //readonly string serverXml = "/Server.xml";
    private JsonData data;
    //服务器资源版本信息
    private Dictionary<string, string> serverResInfoMap = new Dictionary<string, string>();
    //本地资源版本信息
    private Dictionary<string, string> localResInfoMap = new Dictionary<string, string>();
    void Awake()
    {
        Instance = this;
        Loom.Init();
    }

	void Start () {
        DontDestroyOnLoad(gameObject);
        Init();
        //Test();
    }

    void Test()
    {
        string path = Tips.STREAMINGPATH + fileName;
        Debug.Log(Tips.PERSISPATH);
        //if (!File.Exists(path))
        //{
        //    Debug.Log("不存在此文件!!!!!");
        //    return;
        //}
        //var xml = XMLParser.LoadOutter(path);
       
        //foreach (System.Security.SecurityElement item in xml.Children)
        //{
        //    Debug.Log(item.Attribute("name"));
        //}
       
    }

    //初始化
    void Init()
    {
        //Debug.Log(Tips._persisPath + "ResInfo.xml");
        //TestData();
        //获取资源列表
        DownloadMgr.Instance.AsynDownLoadText(Tips.SERVER_SAAS_PATH, AsynResult, OnError);
    }

    private void TestData()
    {
        if (File.Exists(Tips.PERSISPATH + fileName))
        {
            //读取表
            Debug.Log(Tips.PERSISPATH);
            var xml = XMLParser.LoadOutter(Tips.LOCAL_RESINFO_PATH);

            foreach (System.Security.SecurityElement item in xml.Children)
            {
                Debug.Log(item.Attribute("name"));
                localResInfoMap.Add(item.Attribute(Tips.NAME), item.Attribute(Tips.VERSION));
            }
            //下载服务器表，并对比
            DownloadMgr.Instance.AsynDownLoadText(Tips.SERVER_SAAS_PATH, AsynResult, OnError);
        }
        else
        {
            //创建临时数据文件夹
            if (!Directory.Exists(Tips.PERSISPATH))
                Directory.CreateDirectory(Tips.PERSISPATH);
            //查找资源表ResInfo
            string xmlPath = Tips.STREAMINGPATH + fileName;
            //从streamingAssets里获取表，并写入持久化目录
            if (File.Exists(xmlPath))
            {
                //读取资源表
                //Debug.Log("找到啦!!!!!!!!!!!"); 

                //在云平台获取资源列表
                DownloadMgr.Instance.AsynDownLoadText(Tips.SERVER_SAAS_PATH, AsynResult, OnError);
            }
        }
    }

    void AsynResult(string result)
    {
        Debug.Log(result);
        SetData(result, LoadScene);     
    }

    void OnError()
    {
    }

    //创建资源版本文件，记录资源版本
    private void CreateResInfoConfig(string path, List<SaasAir> data)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root = xmlDoc.CreateElement("root");
        xmlDoc.AppendChild(root);
        var it = data.GetEnumerator();
        while (it.MoveNext())
        {
            string id = it.Current.Id;
            string name = it.Current.Name;
            string remark = it.Current.Remark;
            string versionName = it.Current.VersionNum;
            SaasAirType airType = it.Current.AirType;
            string filePath = it.Current.FilePath;
            string bundle = it.Current.Bundle;
            string manifest = it.Current.Manifest;
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
            root.AppendChild(record);
        }
        xmlDoc.Save(path);
    }

    public void SetData(string result, Action action = null)
    {
        serverResult = result;
        data = JsonConvert.DeserializeObject<JsonData>(result);
        //写入持久化目录
        //CreateResInfoConfig(Tips.PERSISPATH + fileName, data.data);
        if (action != null)
            action();
    }

    public List<SaasAir> GetData()
    {
        return data.data;
    }

    private string serverResult;
    public string GetResult()
    {
        return serverResult;
    }

    private void LoadScene()
    {
        Loom.RunOnMainThread(() => {
            SceneManager.LoadScene("Loading");
        });       
    }

}
