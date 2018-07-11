using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Content;
using Util;
using Common;

public class UISceneMain : UIScene {

    private List<UISceneWidget> widges;
    private List<UISceneWidget> listSortedItems = new List<UISceneWidget>();
    private int mCenterIndex = 0;
    private UISceneWidget[] array;
    //public ARController arController;
    public GameObject arCamera;
    private NativeCall native;
    void Awake()
    {
        UIManager.Instance.RegiSecondPanel(gameObject, UnLoad);
    }

    protected override void Start ()
    {
        base.Start();
        UIManager.Instance.SetVisible(UIName.UISceneProgress, true);
        //UIManager.Instance.RegiSecondPanel(gameObject, null);

        //Init();
        //首先检查更新
        //UISceneProcess.Instance.CheckOnStart(() =>
        //{
        //    SetVisible(false);
        //    native = GameObject.Find("NativeCall").GetComponent<NativeCall>();
        //    ContentHelper.Instance.Init();
        //    UIManager.Instance.SetMainVisible(true);
        //    //UIManager.Instance.SetVisible(UIName.UISceneMain, false);
        //    CheckTimeOut.Instance.currRecord = new RecordTime(true, native.GetEyeProtectionDuration() * 60, () =>
        //    {
        //        if (native.IsEyeProtectionToneOn())
        //        {
        //            UIManager.Instance.SetVisible(UIName.UISceneHint, true);
        //            UISceneHint.Instance.SetTextHint("");
        //        }
        //    });
        //    CheckTimeOut.Instance.RegiRecord();
        //    //初始化管理mark
        //    CombineControl.Instance.ARInit();
        //});
    }

    private void UnLoad()
    {

    }

    private void Init()
    {
        widges = new List<UISceneWidget>();
        array = transform.GetComponentsInChildren<UISceneWidget>();
        widges.AddRange(array);
        for (int i = 0; i < widges.Count; i++)
        {
            widges[i].OnMouseClick = WidgeOnClick;
        }
        OnStart();
    }

    private void OnStart()
    {
        
        listSortedItems = new List<UISceneWidget>(array);
        mCenterIndex = listSortedItems.Count % 2 == 0 ? listSortedItems.Count / 2 - 1 : listSortedItems.Count / 2;
        listSortedItems.Sort((p, q) => p.transform.localPosition.x.CompareTo(q.transform.localPosition.x));
        centerVec = listSortedItems[mCenterIndex].transform.localPosition;
        curCenterItem = listSortedItems[mCenterIndex];
        for (int i = 0; i < listSortedItems.Count; i++)
        {
            SetSelectState(listSortedItems[i].gameObject, false);
        }
        SetSelectState(curCenterItem.gameObject, true);
    }


    private UISceneWidget curCenterItem;
    private Vector3 centerVec;
    private void WidgeOnClick(UISceneWidget eventObj)
    {
        if (curCenterItem == eventObj)//进入场景
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.limit = true;
                AudioManager.Instance.Mute();
            }
            //TODO 按钮特效，进入场景
            eventObj.transform.DOScale(1.2f, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(()=> {
                string sceneName = eventObj.name.Substring(eventObj.name.LastIndexOf('_') + 1);
                UIManager.Instance.SetVisible(UIName.UISceneSet, false);
                SetVisible(false);
                UIManager.Instance.SetMainVisible(true);
                //if (arController != null)
                //    arController.VideoBackground.SetActive(true);              
                if (arCamera != null)
                    arCamera.GetComponent<Camera>().enabled = true;


            });
            return;
        }
        curCenterItem = eventObj;

        int count = listSortedItems.Count;
        if (eventObj.transform.localPosition.x > centerVec.x)//在右边
            listSortedItems.Sort((p, q) => p.transform.localPosition.x.CompareTo(q.transform.localPosition.x));
        else//在左边
            listSortedItems.Sort((p, q) => q.transform.localPosition.x.CompareTo(p.transform.localPosition.x));
        float time = 0.4f;
        for (int i = 0; i < listSortedItems.Count; i++)
        {
            int index = i == 0 ? 2 : i - 1;
            listSortedItems[i].transform.SetSiblingIndex(i+1);
            SetSelectState(listSortedItems[i].gameObject, false);
            listSortedItems[i].transform.DOLocalMove(listSortedItems[index].transform.localPosition, time).SetEase(Ease.OutBack);
        }
        SetSelectState(curCenterItem.gameObject, true);
    }

    private void SetSelectState(GameObject obj, bool visible)
    {
        obj.transform.localScale = visible ? Vector3.one * 1.1f : Vector3.one * 0.8f;
    }
}
