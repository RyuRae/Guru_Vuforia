using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using cn.sharerec;
using System.IO;
using System;

public class UISceneTools : UIScene {

    private UISceneWidget mToggle_Tool;
    private Image image_Check;
    private bool isShow = false;
    private GameObject gird;
    private float startValue = 128;
    private float endValue = -128;
    private UISceneWidget mButton_Photo;
    private UISceneWidget mButton_Record;
    private Sprite _sprite;
    private bool IsRecord;
    private int premasking;
    private Camera mCamera;
    private Camera arCamera;
    Rect rect;
    private Camera frontMostCamera;
    //private GameObject videoBack;
    //private ARController arControl;
    private Camera _camera;
    private bool IsRender;
    private NativeCall native;
    protected override void Start () {
        base.Start();
        //arControl = GameObject.Find("SceneObj/ARToolKit").GetComponent<ARController>();
        //videoBack = arControl.VideoBackground;
        //if (videoBack != null)
        //    mCamera = videoBack.GetComponent<Camera>();
        arCamera = GameObject.Find("SceneObj/Scene root/Camera").GetComponent<Camera>();
        mToggle_Tool = GetWidget("Toggle_Tool");
        image_Check = Global.FindChild<Image>(transform, "Image_Check");
        tool = mToggle_Tool.GetComponent<Image>();
        gird = Global.FindChild(transform, "Grid");
        if (mToggle_Tool != null)
            mToggle_Tool.OnMouseClick = ToggleToolOnClick;
        image_Check.enabled = false;
        mButton_Photo = GetWidget("Button_Photo");
        mButton_Record = GetWidget("Button_Record");
        if (mButton_Record != null)
            mButton_Record.OnMouseClick = ButtonRecordOnClick;
        if (mButton_Photo != null)
            mButton_Photo.OnMouseClick = ButtonPhotoOnClick;
        gird.transform.localPosition = new Vector3(60f, 128, 0);
        _sprite = tool.sprite;
        rect = new Rect(0, 0, Screen.width, Screen.height);
        //frontMostCamera = GameObject.Find("FrontMostCamera").GetComponent<Camera>();
        //if (frontMostCamera != null)
        //    frontMostCamera.enabled = false;
        IsRender = false;
        native = GameObject.Find("NativeCall").GetComponent<NativeCall>();
    }

    private Image tool;
    private void ToggleToolOnClick(UISceneWidget eventObj)
    {
        isShow = !isShow;
        if (isShow)
        {
            tool.sprite = image_Check.sprite;
            gird.transform.DOLocalMoveY(endValue, 0.5f);
        }
        else
        {
            tool.sprite = _sprite;
            gird.transform.DOLocalMoveY(startValue, 0.5f);
        }
    }
     
    private void ButtonPhotoOnClick(UISceneWidget eventObj)
    {
        //Debug.Log("开始拍照");
        //if(mCamera == null)
        //    mCamera = arControl.VideoBackground.GetComponent<Camera>();
        //if (_camera == null)
        //    _camera = arControl.gameObject.GetComponent<Camera>();
        //Debug.Log("拍照!");
        Capture(mCamera, arCamera, _camera);
    }

    //获取系统时间并命名相片名 
    private void Capture(Camera camera1, Camera camera2, Camera camera0)
    {
        if (camera1 == null)
        {
            Debug.LogError("相机为空！");
            return;
        }

        //播放拍照声音和UI
        AudioManager.Instance.PlaySystem("photo");
        UIManager.Instance.SetVisible(UIName.UISceneHint, true);
        UISceneHint.Instance.ShowPhotoHint();

        System.DateTime now = System.DateTime.Now;
        string times = now.ToString("yyyy-MM-dd HH:mm:ss");
        times = times.Trim();
        times = times.Replace("/", "-");
        string filename = "Screenshot" + times + ".png";

        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)Screen.width, (int)Screen.height, 24);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        camera0.targetTexture = rt;
        camera0.Render();

        camera1.targetTexture = rt;
        camera1.Render();

        camera2.targetTexture = rt;
        camera2.Render();

        // 激活这个rt, 并从中中读取像素。  
        RenderTexture.active = rt;

        //截取屏幕
        
        Texture2D texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        // 重置相关参数，以使用camera继续在屏幕上显示  
        camera0.targetTexture = null;
        camera1.targetTexture = null;
        camera2.targetTexture = null;
        //ps: camera2.targetTexture = null;  
        RenderTexture.active = null; // JC: added to avoid errors  

        GameObject.Destroy(rt);

        //转为字节数组  
        byte[] bytes = texture.EncodeToPNG();
        string Path_save = null;
        //判断是否为Android平台  
        if (Application.platform == RuntimePlatform.Android)
        {
            string destination = "/sdcard/DCIM/ARphoto";
            //判断目录是否存在，不存在则会创建目录  
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }
            Path_save = destination + "/" + filename;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            Path_save = Application.dataPath + "/Screenshot.png";
        }
        //存图片  
        System.IO.File.WriteAllBytes(Path_save, bytes);
        native.notifyNewMediaAdded(Path_save);
        //if (frontMostCamera != null)
        //    frontMostCamera.depth = 1;
    }
    TimeSpan ts;
    //void Update()
    //{
    //    if (IsCount)
    //    {
    //        if (_time <= 60)
    //        {
    //            _time += Time.deltaTime;
    //            ts = new TimeSpan(0, 0, Convert.ToInt32(_time += Time.deltaTime));
    //            UISceneRecord.Instance.SetTime(_time.ToString("mm:ss"));
    //        }
    //        else
    //        {
    //            IsRecord = false;
    //            Stop();
    //        }
    //    }
    //}

    private bool InitCam()
    {
        if (!IsRender)
        {
            //if (mCamera == null)
            //    mCamera = arControl.VideoBackground.GetComponent<Camera>();
            //if (_camera == null)
            //    _camera = arControl.gameObject.GetComponent<Camera>();
            RenderTexture rt = new RenderTexture((int)Screen.width, (int)Screen.height, 24);
            _camera.targetTexture = rt;
            _camera.Render();
            mCamera.targetTexture = rt;
            mCamera.Render();
            arCamera.targetTexture = rt;
            arCamera.Render();
            RenderTexture.active = rt;
            //ShareREC.addCameraRecord(rt);

            _camera.targetTexture = null;
            mCamera.targetTexture = null;
            arCamera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(rt);
            IsRender = true;
        }
        return IsRender;
    }

    private float _time = 0;
    private bool IsCount;
    private void ButtonRecordOnClick(UISceneWidget eventObj)
    {
        //Debug.Log("录制视频!");
        //IsRecord = !IsRecord;
        //if (IsRecord)
        //{
        //    if (ShareREC.IsAvailable())
        //    {
        //        if (InitCam())
        //        {
        //            //ShareREC.addCameraRecord()
        //            ShareREC.SetVisible(true);
        //            UIManager.Instance.SetVisible(UIName.UISceneRecord, true);
        //            _time = Time.time;
        //            //开始录制视频
        //            ShareREC.OnRecorderStartedHandler = onStarted;
        //            //开始录屏
        //            ShareREC.StartRecorder();                   
        //        }
        //    }
        //    else
        //    {
        //        //显示提示信息
        //        Debug.Log("该设备暂不支持录制视频!");
        //    }
        //}
        //else
        //    Stop();
    }

    public void Stop()
    {
        if (ShareREC.IsAvailable())
        {
            UIManager.Instance.SetVisible(UIName.UISceneRecord, false);
            IsCount = false;
            //停止录制
            ShareREC.OnRecorderStoppedHandler = onStopped;
            ShareREC.StopRecorder();
            //ShareREC.SetVisible(false);
            //if (Time.time - _time >= 4)
            //{
            //    ShareREC.OnRecorderStoppedHandler = onStopped;
            //    ShareREC.StopRecorder();
            //    ShareREC.SetVisible(false);
            //}
            //录制分享
           
        }
    }

    /// <summary>获取录制状态</summary>
    public bool GetRecordStatus()
    {
        return IsRecord;
    }

    //开始录屏时需要改变的状态
    private void onStarted()
    {
        IsCount = true;
    }

    //停止录屏时需要做的事情
    private void onStopped()
    {
        ShareREC.ListLocalVideos();
    }

    //void OnApplicationFocus(bool hasFocus)
    //{
    //    //home到桌面当前hasFocus为false,home进入程序hasFocus为true
    //    //Debug.Log("OnApplicationFocus:" + hasFocus);
    //}

    //void OnApplicationPause(bool pauseStatus)
    //{
    //    //home到桌面当前pauseStatus为true,home进入程序pauseStatus为false
    //    //Debug.Log("OnApplicationPause:" + pauseStatus);
    //}
}
