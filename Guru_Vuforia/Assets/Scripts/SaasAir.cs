using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using System;
using System.Security;

[Serializable]
public class JsonData {

    public int status;
    public string errorMsg;
    public List<SaasAir> data;
}

[Serializable]
public class SaasAirType
{
    private string id;
    private string name;
    private int isDel;
    private DateTime createDate;

    public SaasAirType()
    {

    }


    public SaasAirType(SecurityElement ele)
    {
        this.id = ele.Attribute("id");
        this.name = ele.Attribute("name");
    }
    public SaasAirType(string id, string name)
    {
        this.id = id;
        this.name = name;
    }
    /// <summary>
    /// 类型编号
    /// </summary>
    public string Id
    {
        get { return id; }
        set { id = value; }
    }
    /// <summary>
    /// 类型名称
    /// </summary>
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    /// <summary>
    /// 是否删除
    /// </summary>
    public int IsDel
    {
        get { return isDel; }
        set { isDel = value; }
    }
    /// <summary>
    /// 创建日期
    /// </summary>
    public DateTime CreateDate
    {
        get { return createDate; }
        set { createDate = value; }
    }

    public override string ToString()
    {
        return "SaasAirType{" +
                "id=" + id +
                ", name='" + name + '\'' +
                ", isDel=" + isDel +
                ", createDate=" + createDate +
                '}';
    }

}

[Serializable]
public class SaasAir{

    /// <summary> 唯一id</summary>
    private string id;
   
    private string name;
    
    private string remark;
   
    private string versionNum;
    
    private string bundle;
   
    private string manifest;
    
    private string filePath;
    
    private string bundleFileSize;
    
    private string manifestFileSize;
   
    private SaasAirType airType;
    
    private int sort;
    
    private int isHot;
   
    private DateTime createDate;
    
    private int isDel;

    /// <summary> 唯一id</summary>
    public string Id
    {
        get
        {
            return id;
        }
        set { id = value; }
    }
    /// <summary> 名称</summary>
    public string Name
    {
        get
        {
            return name;
        }
        set { name = value; }
    }
    /// <summary> 描述，500字以内</summary>
    public string Remark
    {
        get
        {
            return remark;
        }
        set { remark = value; }
    }
    /// <summary> 版本号</summary>
    public string VersionNum
    {
        get
        {
            return versionNum;
        }
        set { versionNum = value; }
    }
    /// <summary> bundle文件uuid</summary>
    public string Bundle
    {
        get
        {
            return bundle;
        }
        set { bundle = value; }
    }
    /// <summary> manifest文件uuid</summary>
    public string Manifest
    {
        get
        {
            return manifest;
        }
        set { manifest = value; }
    }
    /// <summary> 文件路径</summary>
    public string FilePath
    {
        get
        {
            return filePath;
        }
        set { filePath = value; }
    }
    /// <summary> 文件大小</summary>
    public string BundleFileSize
    {
        get
        {
            return bundleFileSize;
        }
        set { bundleFileSize = value; }
    }
    /// <summary> 配置文件大小</summary>
    public string ManifestFileSize
    {
        get
        {
            return manifestFileSize;
        }
        set { manifestFileSize = value; }
    }
    /// <summary> bundle文件类型</summary>
    public SaasAirType AirType
    {
        get
        {
            return airType;
        }
        set { airType = value; }
    }
    /// <summary> 排序</summary>
    public int Sort
    {
        get
        {
            return sort;
        }
        set { sort = value; }
    }
    /// <summary> 是否热门</summary>
    public int IsHot
    {
        get
        {
            return isHot;
        }
        set { isHot = value; }
    }
    /// <summary> 创建日期</summary>
    public DateTime CreateDate
    {
        get
        {
            return createDate;
        }
        set { createDate = value; }
    }
    /// <summary> 是否删除</summary>
    public int IsDel
    {
        get
        {
            return isDel;
        }
        set { isDel = value; }
    }
    public SaasAir()
    {

    }

    public SaasAir(string id, string name, string remark, string version, string filePath, string bundle, string mainfest, SaasAirType type)
    {
        this.id = id;
        this.name = name;
        this.remark = remark;
        this.versionNum = version;
        this.filePath = filePath;
        this.bundle = bundle;
        this.manifest = mainfest;
        this.airType = type;
    }
}
