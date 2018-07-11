using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Utils
{
    /************************************
    * Copyright (C) 2017 北京，讯飞幻境
    * 模块名:Utils
    * 创建者：刘磊
    * 修改者列表：
    * 创建日期：2017.11.23
    * 模块描述：bundle资源表
    * **********************************/
    public class ResourceUnit
    {
        public string name;
        public string hash;
        public string bundleName;
        public long bundleSize;

        public ResourceUnit(XmlElement xe)
        {
            name = xe.GetAttribute("name");
            hash = xe.GetAttribute("hash");
            bundleName = xe.GetAttribute("bundleName");
            bundleSize = long.Parse(xe.GetAttribute("size"));
        }

        public ResourceUnit(string name, string bundleName)
        {
            this.name = name;
            this.bundleName = bundleName;
        }
    }


    public class ResourceInfo
    {
        public string version;
        public long MainManifestSize;
        public Dictionary<string, ResourceUnit> units;

        public ResourceInfo()
        {
            version = "local";/*StrConst.STR_LOCAL;*/
            MainManifestSize = 0;
            units = new Dictionary<string, ResourceUnit>();
        }

        //从XML创建
        public ResourceInfo(XmlNode root)
        {
            XmlElement xe = (XmlElement)root;
            version = xe.GetAttribute("version");
            XmlNodeList list = root.ChildNodes;
            units = new Dictionary<string, ResourceUnit>();
            var it = list.GetEnumerator();
            while (it.MoveNext())
            {
                XmlElement xe_unit = (XmlElement)it.Current;
                ResourceUnit au = new ResourceUnit(xe_unit);
                units.Add(au.name, au);
            }
        }

        public void init(XmlNode root)
        {
            XmlElement xe = (XmlElement)root;
            version = xe.GetAttribute("version");
            XmlNodeList list = root.ChildNodes;
            units = new Dictionary<string, ResourceUnit>();
            var it = list.GetEnumerator();
            while (it.MoveNext())
            {
                XmlElement xe_unit = (XmlElement)it.Current;
                ResourceUnit au = new ResourceUnit(xe_unit);
                units.Add(au.name, au);
            }
        }

    }
}
