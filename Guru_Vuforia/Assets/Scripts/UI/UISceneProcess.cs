using Content;
using FileTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UISceneProcess : UIScene {

    public static UISceneProcess Instance;
    public GameObject progressBar;
    public Slider progressBar_Slider;
    public Text progressBar_Descrip;
    public Text progressBar_Percent;
    public Text serverConnection_Text;
    private Transform sceneRoot;
    //private ARController arControl;
    private NativeCall native;
    void Awake()
    {
        Instance = this;
        sceneRoot = GameObject.Find("SceneObj/Scene root").GetComponent<Transform>();
        //arControl = GameObject.Find("SceneObj/ARToolKit").GetComponent<ARController>();
        native = GameObject.Find("NativeCall").GetComponent<NativeCall>();
    }

    private void OnEnable()
    {
        ManagerEvent.Register(ManagerEvent.MSG_DiviceInfo, SetDiviceInfo);
        ManagerEvent.Register(ManagerEvent.MSG_ProgressBar, SetProgressBar);
        ManagerEvent.Register(ManagerEvent.MSG_ServerConnection, SetServerConnection);
    }

    private void OnDisable()
    {
        ManagerEvent.Unregister(ManagerEvent.MSG_DiviceInfo, SetDiviceInfo);
        ManagerEvent.Unregister(ManagerEvent.MSG_ProgressBar, SetProgressBar);
        ManagerEvent.Unregister(ManagerEvent.MSG_ServerConnection, SetServerConnection);
    }

    protected override void Start () {
        base.Start();
        //SetProgressBar(false);
        
    }

    private void SetProgressBar(params object[] args)
    {
        if (args == null) return;
        bool isShow = false;
        float slider = 0;
        string descrip = "";
        for (int i = 0; i < args.Length; i++)
        {
            object obj = args[i];
            switch (i)
            {
                case 0: isShow = (bool)obj; break;
                case 1: slider = (float)obj; break;
                case 2: descrip = (string)obj; break;
            }
        }

        progressBar.SetActive(isShow);
        progressBar_Slider.value = slider;
        progressBar_Descrip.text = descrip;
        progressBar_Percent.text = slider.ToString("0%");
        //if (slider == 1f)
        //{
        //    StopCoroutine(delayClose(progressBar));
        //    StartCoroutine(delayClose(progressBar));
        //}
    }

    private void SetDiviceInfo(params object[] args)
    {


    }

    private void SetServerConnection(params object[] args)
    {
        if (args == null) return;

        string errorString = "";
        if (args[0] is string)
        {
            string temp = (string)args[0];
            if (temp.StartsWith("re"))
            {
                errorString = "连接服务器失败";
            }
            else if (temp.StartsWith("error is 404") ||
                     temp.StartsWith("error is 504") ||
                     temp.StartsWith("error is 502"))
            {
                errorString = "未找到指定文件";
            }
            else
            {
                errorString = "网络连接错误 (>_<)!";
            }

        }
        else if (args[0] is int)
        {
            switch ((int)args[0])
            {
                case 504: errorString = "(504)未在服务器上找到指定文件 (⊙_⊙)？"; break;
                default: errorString = "网络连接错误 (>_<)!"; break;
            }
        }
        serverConnection_Text.text = errorString;
    }



    private IEnumerator delayClose(GameObject go)
    {
        yield return new WaitForSeconds(0.73f);
        go.SetActive(false);
    }

    public void CheckOnStart(Action action = null)
    {
        UpdateFilesManager.Instance.CheckAndUpdate(() =>
        {
            Debug.Log(Application.persistentDataPath);
            ContentHelper.Instance.Init();
            //隐藏当前场景
            UIManager.Instance.SetVisible(UIName.UISceneMain, false);
            //显示返回主界面场景
            UIManager.Instance.SetVisible(UIName.UISceneHome, true);
            UISceneHome.Instance.SetEvent(UIName.UIAirMain);
            //显示选择界面场景
            UIManager.Instance.SetVisible(UIName.UISceneChoose, true);

            //LoadResourcesAndScene(action);
        });
    }

    //List<ARMarker> list = new List<ARMarker>();
    //加载资源并切换场景
    public void LoadResourcesAndScene(Action action = null)
    {
        Dictionary<string, AssetRecord> recordDic = AssetsManager.Instance.RecordsInfo.GetRecordsDic(RuntimeAssetType.BUNDLE_PREFAB);

        Queue<AssetRecord> queue = new Queue<AssetRecord>();

        var it = recordDic.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.Key.EndsWith("_card"))
            {
                queue.Enqueue(it.Current.Value);
            }
        }

        int index = 0;
        
        AssetsManager.Instance.LoadMulti(queue, false, LoadMethod.BUNDLE_FILE, (obj) =>
        {
            if (obj != null)
            {     
                GameObject go = obj as GameObject;
                GameObject _go = Instantiate(go, sceneRoot);
                _go.transform.SetSiblingIndex(index++);
                _go.name = go.name.Replace("_card", "");
                //_go.GetComponent<ARTrackedObject>().MarkerTag = _go.name;
                //if (!(arControl.GetComponent<ARMarker>() != null && arControl.GetComponent<ARMarker>().Tag == _go.name))
                //{
                //    ARMarker marker = arControl.gameObject.AddComponent<ARMarker>();
                //    marker.Tag = _go.name;
                //    marker.MarkerType = MarkerType.NFT;
                //    marker.NFTDataName = _go.name;
                //    if (_go.GetComponent<LetterTrackableEventHandler>() != null)
                //    {                                              
                //        if(_go.name.Contains("_letter"))
                //            _go.name = _go.name.Replace("_letter", "");
                //    }
                //    list.Add(marker);
                //}
            }
            else
            {
                Debug.Log("加载失败");
            }

        }, () =>
        {
            Debug.Log("所有物体加载完毕！！！！！！！！！！！");
            //sceneRoot.GetComponent<AROrigin>().enabled = true;
            //list.ForEach(p => p.Load());
            //arControl.StartAR();
            ////SetVisible(false);          
            //if (action != null)
            //    action();

            //UIManager.Instance.SetVisible(UIName.UISceneMain, false);           
            //StartCoroutine(onLoaded(action));
        });
    }


    //IEnumerator onLoaded(Action action = null)
    //{
    //    sceneRoot.GetComponent<AROrigin>().enabled = true;
    //    Debug.Log("加载mark的数量：" + list.Count);
    //    list.ForEach(p => p.Load());
    //    arControl.StartAR();

    //    while (!(arControl.VideoBackground != null && arControl.VideoBackground.activeSelf))
    //    {
    //        if (native.GetCameraState())
    //            yield return new WaitForEndOfFrame();
    //        else
    //        {
    //            显示提示
    //            UIManager.Instance.SetVisible(UIName.UISceneHint, true);
    //            UISceneHint.Instance.ShowCameraConnectHint();
    //            break;
    //        }
    //    }
    //    if (action != null)
    //        action();

    //}

}
