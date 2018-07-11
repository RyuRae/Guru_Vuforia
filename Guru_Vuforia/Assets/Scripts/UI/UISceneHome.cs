using Common;
using Content;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

public class UISceneHome : UIScene
{
    public static UISceneHome Instance;
    UISceneWidget mButton_Home;
    Text text_log;

    public Action OnClear = null;
    public Action OnStarted = null;
    //public ARController arController;
    public GameObject arCamera;
    //public GameObject uiCamera;
    private Action backEvent = null;
    void Awake()
    {
        Instance = this;
        UIManager.Instance.RegiSecondPanel(gameObject, backEvent);//将此UI场景当做二级界面是为了点击返回时返回主页
    }

    protected override void Start()
    {
        base.Start();
        mButton_Home = GetWidget("Button_Home");
        if (mButton_Home != null)
            mButton_Home.OnMouseClick = ButtonHomeOnClick;
        text_log = Global.FindChild<Text>(transform, "Text_Log");
        //uiCamera = GameObject.Find("UI/Canvas/UICamera");       
    }

    public void SetEvent(string scene)
    {
        if (scene.Equals(UIName.UISceneChoose))
            backEvent = BackHome;
        else if (scene.Equals(UIName.UIAirMain))
            backEvent = BackStart;
        UIManager.Instance.RegiSecondPanel(gameObject, backEvent);
    }

    private void ButtonHomeOnClick(UISceneWidget eventObj)
    {
        var tools = UIManager.Instance.GetUI<UISceneTools>(UIName.UISceneTools);
        if (tools.GetRecordStatus())//停止录制
            tools.Stop();
        backEvent();
        //BackHome();
    }

    //返回主页
    private void BackHome()
    {
        #region 隐藏当前状态
        if (OnClear != null)
            OnClear();
        //if (arController != null && arController.VideoBackground != null)
        //    arController.VideoBackground.SetActive(false);

        if (arCamera != null)
            arCamera.GetComponent<Camera>().enabled = false;
        //UIManager.Instance.SetMainVisible(false);
        SetEvent(UIName.UIAirMain);
        UIManager.Instance.SetVisible(UIName.UISceneTools, false);
        CombineControl.Instance.OnReset();
        CombineControl.Instance.SetVisible(false);
        CheckTimeOut.Instance.UnRegiRecord();
        AudioManager.Instance.PlayUnitReset();
        AudioManager.Instance.Stop();
        UISceneHint.Instance.SetVisible(false);
        #endregion

        #region 切换主界面状态
        //UIManager.Instance.SetVisible(UIName.UISceneSet, true);
        UIManager.Instance.SetVisible(UIName.UISceneChoose, true);
        UISceneChoose.Instance.UnLoad();
        //var main = UIManager.Instance.GetUI<UIAirMain>(UIName.UIAirMain);
        //main.SetStatus(true);
        //AudioManager.Instance.Replay();
        //AudioManager.Instance.limit = false;
        //if (uiCamera != null)
        //    uiCamera.SetActive(true);
        #endregion
    }

    private void BackStart()
    {
        #region 隐藏当前状态
        UIManager.Instance.SetVisible(UIName.UISceneChoose, false);
        SetVisible(false);
        #endregion
        #region 切换开始界面状态
        UIManager.Instance.SetVisible(UIName.UIAirMain, true);
        UIManager.Instance.SetVisible(UIName.UISceneSet, true);
        AudioManager.Instance.Replay();
        AudioManager.Instance.limit = false;
        #endregion
    }

    public void SetText(string text)
    {
        if (text_log != null)
            text_log.text = text;
    }
}
