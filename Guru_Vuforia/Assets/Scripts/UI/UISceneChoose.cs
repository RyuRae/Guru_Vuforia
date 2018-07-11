using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DevelopEngine;
using Content;
using FileTools;
using Common;
using DG.Tweening;

public class UISceneChoose : UIScene
{
    public static UISceneChoose Instance;

    private UISceneWidget[] widgets;
    private Transform sceneRoot;
    //private ARController arControl;
    private NativeCall native;
    private Camera arCamera;
    void Awake()
    {
        Instance = this;
        sceneRoot = GameObject.Find("SceneObj/Scene root").GetComponent<Transform>();
        //arControl = GameObject.Find("SceneObj/ARToolKit").GetComponent<ARController>();
        native = GameObject.Find("NativeCall").GetComponent<NativeCall>();
        arCamera = Global.FindChild<Camera>(sceneRoot.transform, "Camera");
    }
    private GameObject choose;
    private UISceneWidget image_Crab;
    private UISceneWidget image_HotBalloon;
    private Vector3 initPos;
    DOTweenPath tween;
    protected override void Start()
    {
        base.Start();
        choose = Global.FindChild(transform, "Choose");
        widgets = choose.GetComponentsInChildren<UISceneWidget>();
        for (int i = 0; i < widgets.Length; i++)
        {
            widgets[i].OnMouseClick += OnButtonClick;
        }
        image_Crab = Global.FindChild<UISceneWidget>(transform, "Image_Crab");
        image_HotBalloon = Global.FindChild<UISceneWidget>(transform, "Image_HotBalloon");
        if (image_Crab != null)
        {
            initPos = image_Crab.transform.localPosition;
            tween = image_Crab.GetComponent<DOTweenPath>();
            image_Crab.OnMouseClick = OnImageCrabClick;
        }
        if (image_HotBalloon != null)
            image_HotBalloon.OnMouseClick = OnImageBalloonClick;
    }
    public void Init()
    {
        if (image_Crab != null)
        {
            image_Crab.DORewind();
            image_Crab.transform.localPosition = initPos;
            tween.DORestart();
        }
    }

    public void OnTweenPlay()
    {
        image_Crab.transform.DOLocalRotate(new Vector3(0, 0, 5), 0.1f).SetLoops(4, LoopType.Yoyo);
    }

    private void OnButtonClick(UISceneWidget eventObj)
    {
        if (eventObj.GetComponent<EnhanceItem>().IsCenter() == eventObj.name)
        {
            if (Application.platform == RuntimePlatform.Android && !native.GetCameraState())
            {
                //显示相机连接提示
                UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                UISceneHint.Instance.ShowCameraConnectHint();
                return;
            }
            string str = eventObj.name.Substring(eventObj.name.IndexOf('_') + 1);
            //string tag = Configuration.GetContent(Tips.TAGS, str);
            var list = ContentHelper.Instance.GetListByTag(str);
            UIManager.Instance.SetVisible(UIName.UISceneMain, true);
            UIManager.Instance.SetVisible(UIName.UISceneHome, false);
            LoadResourcesAndScene(list, () =>
            {
                SetVisible(false);
                UIManager.Instance.SetVisible(UIName.UISceneMain, false);
                UIManager.Instance.SetMainVisible(true);
                UISceneHome.Instance.SetEvent(UIName.UISceneChoose);
                int checkTime = native.GetEyeProtectionDuration();
                CheckTimeOut.Instance.currRecord = new RecordTime(true, checkTime * 60, () =>
                {
                    if (native.IsEyeProtectionToneOn())
                    {
                        UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                        UISceneHint.Instance.SetTextHint(checkTime.ToString());
                    }
                });
                CheckTimeOut.Instance.RegiRecord();
                //初始化管理mark
                CombineControl.Instance.ARInit();
                if (!arCamera.enabled)
                    arCamera.enabled = true;
            });
        }
        else
            eventObj.GetComponent<EnhanceItem>().OnClickScrollViewItem(eventObj.gameObject);
    }

    private void OnImageCrabClick(UISceneWidget eventObj)
    {
        tween.DOPause();
        image_Crab.DORewind();
        image_Crab.transform.DOLocalMove(new Vector3(image_Crab.transform.localPosition.x, -1000, 0), 0.5f).OnComplete(() => {
            StartCoroutine(Wait(3, () => {
                image_Crab.transform.DOLocalMove(initPos, 1).OnComplete(() => {
                    tween.DORestart();
                });
            }));
        });
    }

    private void OnImageBalloonClick(UISceneWidget eventObj)
    {
        image_HotBalloon.DORewind();
        image_HotBalloon.transform.localScale = Vector3.one;
        image_HotBalloon.transform.DOShakeScale(1.2f, 0.5f);
    }

    //List<ARMarker> list = new List<ARMarker>();
    //根据场景加载资源加载资源
    private void LoadResourcesAndScene(List<string> nameList, Action action = null)
    {
        int index = 0;
        AssetsManager.Instance.LoadMulti(RuntimeAssetType.BUNDLE_PREFAB, nameList, false, LoadMethod.BUNDLE_FILE, (obj) =>
        {
            if (obj != null)
            {
                GameObject go = obj as GameObject;
                string _name = go.name.Replace("_card", "");
                //ARTrackedObject currGo = Global.FindChild<ARTrackedObject>(sceneRoot.transform, _name);
                //ARMarker marker = null;
                //if (currGo == null)
                //{
                //    GameObject _go = Instantiate(go, sceneRoot);
                //    _go.transform.SetSiblingIndex(index++);
                //    _go.name = go.name.Replace("_card", "");
                //    _go.GetComponent<ARTrackedObject>().MarkerTag = _go.name;
                //    currGo = _go.GetComponent<ARTrackedObject>();
                //}
                //if (!currGo.gameObject.activeSelf)
                //    currGo.gameObject.SetActive(true);
                //if (!(arControl.GetComponent<ARMarker>() != null && arControl.GetComponent<ARMarker>().Tag == currGo.name))
                //{
                //    marker = arControl.gameObject.AddComponent<ARMarker>();
                //    marker.Tag = currGo.name;
                //    marker.MarkerType = MarkerType.NFT;
                //    marker.NFTDataName = currGo.name;
                //    marker.NFTScale = 25f;
                //    if (currGo.GetComponent<LetterTrackableEventHandler>() != null)
                //    {
                //        if (currGo.name.Contains("_letter"))
                //            currGo.name = currGo.name.Replace("_letter", "");
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

            //StartCoroutine(onLoaded(action));
        });
    }

    IEnumerator Wait(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    //IEnumerator onLoaded(Action action = null)
    //{
    //    list.ForEach(p => p.Load());
    //    arControl.StartAR();
    //    if (sceneRoot.GetComponent<AROrigin>() == null)
    //    {
    //        var orgin = sceneRoot.gameObject.AddComponent<AROrigin>();
    //        orgin.findMarkerMode = AROrigin.FindMode.AutoAll;
    //    }
    //    while (!(arControl.VideoBackground != null && arControl.VideoBackground.activeSelf))
    //    {
    //        if(Application.platform == RuntimePlatform.Android && !native.GetCameraState())
    //        {
    //            //显示提示
    //            UIManager.Instance.SetVisible(UIName.UISceneHint, true);
    //            UISceneHint.Instance.ShowCameraConnectHint();
    //            break;
    //        }

    //        yield return new WaitForEndOfFrame();
    //        //if (native.GetCameraState())
    //        //    yield return new WaitForEndOfFrame();
    //        //else
    //        //{
    //        //    //显示提示
    //        //    UIManager.Instance.SetVisible(UIName.UISceneHint, true);
    //        //    UISceneHint.Instance.ShowCameraConnectHint();
    //        //    break;
    //        //}
    //    }
    //    if (action != null)
    //        action();
    //}

    public void UnLoad()
    {
       
        //arControl.StopAR();
        ////sceneRoot.GetComponent<AROrigin>().enabled = false;
        //list.ForEach(p =>
        //{
        //    p.Unload();
        //    Destroy(p);
        //});
        //var orgin = sceneRoot.GetComponent<AROrigin>();
        //if (orgin != null)
        //    Destroy(orgin);
        //list.Clear();
    }
}
