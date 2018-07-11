using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISceneHint : UIScene {

    public static UISceneHint Instance;
    public List<Sprite> hints;
    private Text text_Hint;
    private GameObject image_Intro;
    private GameObject image_HealthHint;
    private NativeCall native;
    private UISceneWidget mButton_CameraSure;
    private UISceneWidget mButton_NetworkSure;
    private GameObject image_CameraConnect;
    private GameObject currHint;//当前提示
    private GameObject unlock;
    private GameObject image_NetworkConnect;

    private GameObject image_bullet1;
    private GameObject image_bullet2;
    private GameObject image_Statement;
    private GameObject image_Photo;
    private static Color _color = new Color(1, 1, 1, 0);
    private static Color mColor = new Color(1, 1, 1, 1);
    void Awake()
    {
        Instance = this;
        text_Hint = Global.FindChild<Text>(transform, "Text_Hint");
        image_Intro = Global.FindChild(transform, "Image_Intro");
        image_HealthHint = Global.FindChild(transform, "Image_HealthHint");
        image_CameraConnect = Global.FindChild(transform, "Image_CameraConnect");
        unlock = Global.FindChild(transform, "Unlock");
        image_NetworkConnect = Global.FindChild(transform, "Image_NetWorkConnect");

        image_bullet1 = Global.FindChild(transform, "Image_bullet1");
        image_bullet2 = Global.FindChild(transform, "Image_bullet2");
        image_Statement = Global.FindChild(transform, "Image_Statement");
        image_Photo = Global.FindChild(transform, "Image_Photo");

        UIManager.Instance.RegiSecondPanel(image_CameraConnect, SetBack);
        UIManager.Instance.RegiSecondPanel(unlock, UnlockHint);
        UIManager.Instance.RegiSecondPanel(image_NetworkConnect, BackStart);
    }

    //void OnEnable()
    //{
    //    RegiSecondPanel(image_Connect, SetBack);
    //    RegiSecondPanel(unlock, UnlockHint);
    //}

    //void OnDisable()
    //{
    //    UnRegiSecondPanel(image_Connect);
    //    UnRegiSecondPanel(unlock);
    //}

    protected override void Start () {
        base.Start();
        native = GameObject.Find("NativeCall").GetComponent<NativeCall>();
        mButton_CameraSure = GetWidget("Button_CameraSure");
        if (mButton_CameraSure != null)
            mButton_CameraSure.OnMouseClick = OnButtonCameraSureClick;
        mButton_NetworkSure = GetWidget("Button_NetWorkSure");
        if (mButton_NetworkSure != null)
            mButton_NetworkSure.OnMouseClick = OnButtonNetworkSureClick;
    }

    public void SetTextHint(string text)
    {       
        image_HealthHint.SetActive(true);
        if (native == null)
            native = GameObject.Find("NativeCall").GetComponent<NativeCall>();
        AudioManager.Instance.PlaySystem("healthhint_" + native.GetEyeProtectionDuration().ToString());
        if (text_Hint != null)
            text_Hint.text = text;
        StartCoroutine(WaitFor(3, () => {
            if (image_HealthHint != null)
                image_HealthHint.SetActive(false);
        }));//3s之后隐藏
    }

    public void SetImageIntro()
    {
        if(image_Intro == null)
            image_Intro = Global.FindChild(transform, "Image_Intro");
        image_Intro.SetActive(true);
        StartCoroutine(WaitFor(3, ()=> {
            if (image_Intro != null)
                image_Intro.SetActive(false);
        }));//3s之后隐藏
    }

    IEnumerator WaitFor(float time, Action action = null)
    {
        yield return new WaitForSeconds(time);
        
        if (action != null)
            action();
        SetVisible(false);
    }

    /// <summary>
    /// 大于五张字母卡牌的弹窗
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    /// 
    public void SetBullet(string str)
    {
        if (str == "Image_bullet1")
        {
            if (image_bullet1 == null)
                image_bullet1 = Global.FindChild(transform, "Image_bullet1");
            image_bullet1.SetActive(true);
            StartCoroutine(WaitFor(3,()=> {
                image_bullet1.SetActive(false);
            }));//3s之后隐藏
        }
        if (str == "Image_bullet2")
        {
            if (image_bullet2 == null)
                image_bullet2 = Global.FindChild(transform, "Image_bullet2");
            image_bullet2.SetActive(true);
            StartCoroutine(WaitFor(3,()=> {
                image_bullet2.SetActive(true);
            }));//3s之后隐藏
        }
    }

    public void ShowCameraConnectHint()
    {
        if (image_CameraConnect != null)
        {
            currHint = image_CameraConnect;
            image_CameraConnect.SetActive(true);
        }
    }

    public void ShowStatementHint(string hint)
    {       
        if (image_Statement != null)
        {
           
            currHint = image_Statement;
            var sprite = hints.Find(p => p.name.Equals(hint));
            if (sprite != null)
            {
             
                var image = Global.FindChild<Image>(image_Statement.transform, "Hint");// image_Statement.GetComponent<Image>();
                image.sprite = sprite;
                image.SetNativeSize();
            }
            image_Statement.SetActive(true);
            AudioManager.Instance.PlaySystem(hint, () => {
                image_Statement.SetActive(false);
                SetVisible(false);
                //StartCoroutine(WaitFor(1, () => {
                //    image_Statement.SetActive(false);
                //}));//3s之后隐藏
            });
           
        }
    }


    public void ShowPhotoHint()
    {
        if (image_Photo != null)
        {
            image_Photo.SetActive(true);
            var image = image_Photo.GetComponent<Image>();
            image.color = _color;
            image.DOColor(mColor, 0.3f).SetLoops(2,LoopType.Yoyo).OnComplete(() => {
                image_Photo.SetActive(false);
                SetVisible(false);
            });
        }
    }


    private void OnButtonCameraSureClick(UISceneWidget eventObj)
    {
        SetBack();
    }

    //设置返回
    private void SetBack()
    {
        if (currHint != null && currHint.activeSelf)
        {
            currHint.SetActive(false);
            currHint = null;
        }
        SetVisible(false);
    }

    public void ShowNetworkConnectHint()
    {
        if (image_NetworkConnect != null)
        {
            currHint = image_NetworkConnect;
            image_NetworkConnect.SetActive(true);
        }
    }

    private void OnButtonNetworkSureClick(UISceneWidget eventObj)
    {
        BackStart();
    }

    private void BackStart()
    {
        SetBack();
        #region 隐藏当前状态
        SetVisible(false);
        #endregion
        #region 切换开始界面状态
        UIManager.Instance.SetVisible(UIName.UIAirMain, true);
        UIManager.Instance.SetVisible(UIName.UISceneSet, true);
        AudioManager.Instance.Replay();
        AudioManager.Instance.limit = false;
        #endregion
    }

    public void ShowUnlockHint()
    {
        if (unlock != null)
            unlock.SetActive(true);
    }

    public void UnlockHint()
    {
        if (unlock != null)
            unlock.SetActive(false);
        SetVisible(false);
    }
}
