using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetFileList : MonoBehaviour {

    private bool isInit = false;
    private static AssetFileList _instace = null;

    public static AssetFileList Instace
    {
        get
        {
            if (_instace == null)
            {
                GameObject go = new GameObject("AssetFileList");
                _instace = go.AddComponent<AssetFileList>();

                _instace.init(() => { });
            }
            return _instace;
        }
    }

    private Action _completeFunc;
    //===================  唯一入口  ===================
    public void init(Action completeFunc, bool reInit = false)
    {

    }
}
